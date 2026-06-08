namespace football_management_system_cscb.Models.Season
{
    public class LeagueTableEntry
    {
        public int Id { get; set; }   // 👈 THIS is required (primary key)

        public int TeamId { get; set; }
        public Team Team { get; set; } = null!;

        public int Played { get; set; }
        public int Wins { get; set; }
        public int Draws { get; set; }
        public int Losses { get; set; }

        public int GoalsFor { get; set; }
        public int GoalsAgainst { get; set; }

        public int Points { get; set; }

        public int SeasonId { get; set; }
        public Season Season { get; set; } = null!;
    }
}