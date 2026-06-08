using football_management_system_cscb.Models;

public class Squad
{
    public Team Team { get; set; }

    public List<Player> StartingXI { get; set; } = new();

    public Squad(Team team)
    {
        Team = team;
    }

    public void AutoSelect()
    {
        StartingXI = Team.Players
            .OrderByDescending(p => p.OverallRating ?? 50)
            .Take(11)
            .ToList();
    }

    public double Strength =>
        StartingXI.Average(p => p.OverallRating ?? 50);



}