using football_management_system_cscb.Data;
using Microsoft.AspNetCore.Mvc;

namespace football_management_system_cscb.Controllers
{
    public class HomeController : Controller
    {
        private readonly FootballDbContext _context;
        private readonly TeamFormService _teamFormService;
        public HomeController(
            FootballDbContext context,
            TeamFormService teamFormService)
        {
            _context = context;
            _teamFormService = teamFormService;
        }

        public IActionResult Index()
        {
            ViewBag.TotalPlayers = _context.Players.Count();
            ViewBag.TotalTeams = _context.Teams.Count();
            ViewBag.TotalMatches = _context.FootballMatches.Count();
            ViewBag.LeagueTable = _teamFormService.GetLeagueTable();

            ViewBag.TotalGoals = _context.FootballMatches
                .Sum(m => m.HomeScore + m.AwayScore);

            var model = _teamFormService.GetTeamsForm();

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}