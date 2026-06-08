namespace football_management_system_cscb.ViewModels.Season
{
    public class FixtureViewModel
    {
        public int Week { get; set; }

        public string HomeTeamName { get; set; } = "";
        public string AwayTeamName { get; set; } = "";

        public int? HomeGoals { get; set; }
        public int? AwayGoals { get; set; }

        public bool Played => HomeGoals.HasValue && AwayGoals.HasValue;
    }
}