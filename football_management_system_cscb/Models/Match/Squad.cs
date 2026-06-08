using football_management_system_cscb.Models;
using football_management_system_cscb.Models.Formation;

public class Squad
{
    public Team Team { get; set; }
    public Formation Formation { get; set; }

    public List<Player> StartingXI { get; set; } = new();
    public List<Player> Bench { get; set; } = new();

    public Squad(Team team, Formation formation)
    {
        Team = team;
        Formation = FormationLibrary.Get(team.DefaultFormation);
    }

    public void BuildSquad(List<Player> players)
    {
        // basic ordering: strongest players first
        var ordered = players
            .OrderByDescending(p => p.OverallRating ?? 50)
            .ToList();

        int gk = Formation.Goalkeepers;
        int def = Formation.Defenders;
        int mid = Formation.Midfielders;
        int att = Formation.Attackers;

        StartingXI.Clear();
        Bench.Clear();

        StartingXI.AddRange(ordered.Take(gk + def + mid + att));
        Bench.AddRange(ordered.Skip(gk + def + mid + att));
    }
}