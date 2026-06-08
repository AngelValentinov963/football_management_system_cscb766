namespace football_management_system_cscb.ViewModels.Season
{
    public class FixtureViewModel
    {
        public int FixtureId {  get; set; }
        public int HomeTeamId { get; set; }
        public int AwayTeamId { get; set; }

        public string HomeTeamName { get; set; } = "";
        public string AwayTeamName { get; set; } = "";

        public string HomeTeamLogoUrl { get; set; } = "";
        public string AwayTeamLogoUrl { get; set; } = "";

        public int? HomeGoals { get; set; }
        public int? AwayGoals { get; set; }

        public bool Played => HomeGoals.HasValue && AwayGoals.HasValue;
    }
}