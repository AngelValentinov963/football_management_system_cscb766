using football_management_system_cscb.ViewModels.Season;

namespace football_management_system_cscb.ViewModel
{
    public class WeeklyFixturesViewModel
    {
        public int WeekNumber { get; set; }
        public List<FixtureViewModel> Fixtures { get; set; } = new();
    }
}
