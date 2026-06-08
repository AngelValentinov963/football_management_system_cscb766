using football_management_system_cscb.Data;
using football_management_system_cscb.Models.Season;
using football_management_system_cscb.ViewModel;

public class TeamFormService
{
    private readonly FootballDbContext _context;

    public TeamFormService(FootballDbContext context)
    {
        _context = context;
    }
    public List<LeagueTableRowViewModel> GetLeagueTable()
    {
        var teams = _context.Teams.ToList();

        var fixtures = _context.Fixtures
            .Where(f =>
                f.Played &&
                f.HomeGoals.HasValue &&
                f.AwayGoals.HasValue)
            .ToList();

        var table = new List<LeagueTableRowViewModel>();

        foreach (var team in teams)
        {
            var row = new LeagueTableRowViewModel
            {
                TeamId = team.TeamId,
                TeamName = team.Name,
                LogoUrl = team.LogoUrl
            };

            var teamFixtures = fixtures
                .Where(f =>
                    f.HomeTeamId == team.TeamId ||
                    f.AwayTeamId == team.TeamId);

            foreach (var f in teamFixtures)
            {
                bool isHome = f.HomeTeamId == team.TeamId;

                int gf = isHome
                    ? f.HomeGoals!.Value
                    : f.AwayGoals!.Value;

                int ga = isHome
                    ? f.AwayGoals!.Value
                    : f.HomeGoals!.Value;

                row.Played++;
                row.GoalsFor += gf;
                row.GoalsAgainst += ga;

                if (gf > ga)
                    row.Wins++;
                else if (gf < ga)
                    row.Losses++;
                else
                    row.Draws++;
            }

            table.Add(row);
        }

        return table
            .OrderByDescending(t => t.Points)
            .ThenByDescending(t => t.GoalDifference)
            .ThenByDescending(t => t.GoalsFor)
            .ToList();
    }
    public List<TeamFormViewModel> GetTeamsForm(int lastN = 5)
    {
        var teams = _context.Teams.ToList();

        var fixtures = _context.Fixtures
            .Where(f => f.Played == true &&
                        f.HomeGoals != null &&
                        f.AwayGoals != null)
            .ToList();
       
        var result = new List<TeamFormViewModel>();

        foreach (var team in teams)
        {
            var form = fixtures
                .Where(f => f.HomeTeamId == team.TeamId || f.AwayTeamId == team.TeamId)
                .OrderByDescending(f => f.Id) // or Week / Date if you have it
                .Take(lastN)
                .Select(f => GetResult(f, team.TeamId))
                .ToList();

            result.Add(new TeamFormViewModel
            {
                Name = team.Name,
                Form = form
            });
        }

        return result;
    }

    private string GetResult(Fixture f, int teamId)
    {
        bool isHome = f.HomeTeamId == teamId;

        int goalsFor = isHome ? f.HomeGoals.Value : f.AwayGoals.Value;
        int goalsAgainst = isHome ? f.AwayGoals.Value : f.HomeGoals.Value;

        if (goalsFor > goalsAgainst) return "w";
        if (goalsFor < goalsAgainst) return "l";
        return "d";
    }


    public (int wins, int draws, int losses) GetTeamStats(int teamId)
    {
        var fixtures = _context.Fixtures
            .Where(f => f.Played == true &&
                        f.HomeGoals != null &&
                        f.AwayGoals != null &&
                        (f.HomeTeamId == teamId || f.AwayTeamId == teamId))
            .ToList();

        int w = 0, d = 0, l = 0;

        foreach (var f in fixtures)
        {
            bool isHome = f.HomeTeamId == teamId;

            int gf = isHome ? f.HomeGoals.Value : f.AwayGoals.Value;
            int ga = isHome ? f.AwayGoals.Value : f.HomeGoals.Value;

            if (gf > ga) w++;
            else if (gf < ga) l++;
            else d++;
        }

        return (w, d, l);
    }
}