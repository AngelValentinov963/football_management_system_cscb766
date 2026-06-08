using football_management_system_cscb.Data;
using football_management_system_cscb.Models.Formation;
using football_management_system_cscb.Models.Season;
using football_management_system_cscb.Models;
using football_management_system_cscb.Services;

namespace football_management_system_cscb.Services
{
    public class SeasonService
    {
        private readonly FootballDbContext _context;
        private readonly SquadService _squadService;
        private readonly MatchEngine _matchEngine;

        public SeasonService(
            FootballDbContext context,
            SquadService squadService,
            MatchEngine matchEngine)
        {
            _context = context;
            _squadService = squadService;
            _matchEngine = matchEngine;
        }

        // ==========================
        // FIXTURE GENERATION
        // ==========================
        public List<Fixture> GenerateSeasonFixtures(List<int> teamIds, int seasonId)
        {
            var fixtures = new List<Fixture>();
            var teams = new List<int>(teamIds);

            if (teams.Count % 2 != 0)
                teams.Add(-1);

            int n = teams.Count;
            int rounds = n - 1;

            var rotation = new List<int>(teams);
            int week = 1;

            for (int round = 0; round < rounds; round++)
            {
                for (int i = 0; i < n / 2; i++)
                {
                    int home = rotation[i];
                    int away = rotation[n - 1 - i];

                    if (home == -1 || away == -1)
                        continue;

                    fixtures.Add(new Fixture
                    {
                        Week = week,
                        HomeTeamId = home,
                        AwayTeamId = away,
                        SeasonId = seasonId
                    });
                }

                Rotate(rotation);
                week++;
            }

            // second leg
            var firstLeg = fixtures.ToList();

            foreach (var f in firstLeg)
            {
                fixtures.Add(new Fixture
                {
                    Week = f.Week + rounds,
                    HomeTeamId = f.AwayTeamId,
                    AwayTeamId = f.HomeTeamId,
                    SeasonId = seasonId
                });
            }

            return fixtures;
        }

        // ==========================
        // WEEK SIMULATION (FIXED)
        // ==========================
        public void SimulateWeek(int weekNumber)
        {
            var fixtures = _context.Fixtures
                .Where(f =>
                    f.Week == weekNumber &&
                    !f.HomeGoals.HasValue &&
                    !f.AwayGoals.HasValue)
                .ToList();

            foreach (var fixture in fixtures)
            {
                var homeTeam = _context.Teams.First(t => t.TeamId == fixture.HomeTeamId);
                var awayTeam = _context.Teams.First(t => t.TeamId == fixture.AwayTeamId);

                // ✅ convert DB string → Formation object (CRITICAL FIX)
                Formation homeFormation = FormationLibrary.Get(homeTeam.DefaultFormation);
                Formation awayFormation = FormationLibrary.Get(awayTeam.DefaultFormation);

                var homeSquad = _squadService.GetSquad(homeTeam.TeamId, homeFormation);
                var awaySquad = _squadService.GetSquad(awayTeam.TeamId, awayFormation);

                var result = _matchEngine.SimulateInstantMatch(homeSquad, awaySquad);

                fixture.HomeGoals = result.HomeGoals;
                fixture.AwayGoals = result.AwayGoals;
                fixture.Played = true;

            }

            _context.SaveChanges();
        }

        // ==========================
        // SIMULATE ALL
        // ==========================
        public void SimulateAllUnplayedMatches()
        {
            var fixtures = _context.Fixtures
                .Where(f =>
                    !f.HomeGoals.HasValue &&
                    !f.AwayGoals.HasValue)
                .ToList();

            foreach (var fixture in fixtures)
            {
                var homeTeam = _context.Teams.First(t => t.TeamId == fixture.HomeTeamId);
                var awayTeam = _context.Teams.First(t => t.TeamId == fixture.AwayTeamId);

                Formation homeFormation = FormationLibrary.Get(homeTeam.DefaultFormation);
                Formation awayFormation = FormationLibrary.Get(awayTeam.DefaultFormation);

                var homeSquad = _squadService.GetSquad(homeTeam.TeamId, homeFormation);
                var awaySquad = _squadService.GetSquad(awayTeam.TeamId, awayFormation);

                var result = _matchEngine.SimulateInstantMatch(homeSquad, awaySquad);

                fixture.HomeGoals = result.HomeGoals;
                fixture.AwayGoals = result.AwayGoals;
                fixture.Played = true;

            }

            _context.SaveChanges();
        }

        public void ResetFixtures()
        {
            var fixtures = _context.Fixtures.ToList();
            _context.Fixtures.RemoveRange(fixtures);
            _context.SaveChanges();
        }

        public void ResetAndGenerateSeason(int seasonId)
        {
            // 1. Delete old fixtures
            _context.Fixtures.RemoveRange(_context.Fixtures);
            _context.SaveChanges();

            // 2. Get all teams
            var teamIds = _context.Teams
                .Select(t => t.TeamId)
                .ToList();

            // 3. Generate new fixtures
            var fixtures = GenerateSeasonFixtures(teamIds, seasonId);

            // 4. Save new fixtures
            _context.Fixtures.AddRange(fixtures);
            _context.SaveChanges();
        }

        // ==========================
        // ROTATION
        // ==========================
        private void Rotate(List<int> list)
        {
            int last = list[^1];
            list.RemoveAt(list.Count - 1);
            list.Insert(1, last);
        }
    }
}