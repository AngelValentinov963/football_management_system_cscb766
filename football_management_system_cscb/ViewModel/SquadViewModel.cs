using football_management_system_cscb.ViewModels;

namespace football_management_system_cscb.ViewModel
{
    public class SquadViewModel
    {
        public int TeamId { get; set; }   // ✅ ADD THIS

        public List<PlayerViewModel> Squad { get; set; } = new();
        public List<PlayerViewModel> StartingXI { get; set; } = new();
        public string Formation { get; set; } = "4-3-3";
    }
}
