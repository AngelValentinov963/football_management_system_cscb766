namespace football_management_system_cscb.Services
{
    public class TeamService
    {
        public double GetTeamStrength(Squad squad)
        {
            return squad.StartingXI.Average(p =>
                ((p.OverallRating ?? 50) +
                 p.Pace +
                 p.Shooting +
                 p.Passing +
                 p.Dribbling +
                 p.Defense +
                 p.Stamina) / 7.0);
        }
    }
}
