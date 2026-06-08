namespace football_management_system_cscb.Models.Season
{
    public class Fixture
    {
        public int Id { get; set; }

        public int Week { get; set; }

        public int HomeTeamId { get; set; }
        public Team HomeTeam { get; set; } = null!;

        public int AwayTeamId { get; set; }
        public Team AwayTeam { get; set; } = null!;

        public int? HomeGoals { get; set; }
        public int? AwayGoals { get; set; }

        public bool Played { get; set; } = false;

        public int SeasonId { get; set; }
        public Season Season { get; set; } = null!;

    }
}