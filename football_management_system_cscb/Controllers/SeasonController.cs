using football_management_system_cscb.Data;
using football_management_system_cscb.Models.Season;
using football_management_system_cscb.Services;
using football_management_system_cscb.ViewModel;
using football_management_system_cscb.ViewModels.Season;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace football_management_system_cscb.Controllers
{
    public class SeasonController : Controller
    {
        private readonly FootballDbContext _db;
        private readonly SeasonService _seasonService;

        public SeasonController(FootballDbContext db, SeasonService seasonService)
        {
            _db = db;
            _seasonService = seasonService;
        }

        // =========================
        // FIXTURES VIEW
        // =========================
        public IActionResult Fixtures()
        {
            var season = _db.Seasons
                .Include(s => s.Fixtures)
                    .ThenInclude(f => f.HomeTeam)
                .Include(s => s.Fixtures)
                    .ThenInclude(f => f.AwayTeam)
                .FirstOrDefault();

            // CREATE SEASON IF NONE EXISTS
            if (season == null)
            {
                season = new Season();
                _db.Seasons.Add(season);
                _db.SaveChanges();

                var teamIds = _db.Teams.Select(t => t.TeamId).ToList();

                // FIXED: pass seasonId correctly
                var fixtures = _seasonService.GenerateSeasonFixtures(teamIds, season.Id);

                _db.Fixtures.AddRange(fixtures);
                _db.SaveChanges();

                season = _db.Seasons
                    .Include(s => s.Fixtures)
                        .ThenInclude(f => f.HomeTeam)
                    .Include(s => s.Fixtures)
                        .ThenInclude(f => f.AwayTeam)
                    .First();
            }

            var model = season.Fixtures
                .GroupBy(f => f.Week)
                .OrderBy(g => g.Key)
                .Select(g => new WeeklyFixturesViewModel
                {
                    WeekNumber = g.Key,
                    Fixtures = g.Select(f => new FixtureViewModel
                    {
                        Week = f.Week,
                        HomeTeamName = f.HomeTeam.Name,
                        AwayTeamName = f.AwayTeam.Name,
                        HomeGoals = f.HomeGoals,
                        AwayGoals = f.AwayGoals
                    }).ToList()
                })
                .ToList();

            return View(model);
        }
        // =========================
        // GENERATE SEASON (NEW ACTION)
        // =========================
        [HttpPost]
        public IActionResult GenerateSeason()
        {
            // prevent duplicate seasons
            if (_db.Seasons.Any())
                return RedirectToAction("Fixtures");

            var season = new Season();
            _db.Seasons.Add(season);
            _db.SaveChanges();

            var teamIds = _db.Teams.Select(t => t.TeamId).ToList();

            var fixtures = _seasonService.GenerateSeasonFixtures(teamIds, season.Id);

            _db.Fixtures.AddRange(fixtures);
            _db.SaveChanges();

            return RedirectToAction("Fixtures");
        }
    }
}