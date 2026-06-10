using football_management_system_cscb.Models;
using football_management_system_cscb.ViewModels;

namespace football_management_system_cscb.ViewModels
{
    public class MarketViewModel
    {
        public Team Team { get; set; }

        public List<PlayerViewModel> MyPlayers { get; set; } = new();
        public List<PlayerViewModel> AvailablePlayers { get; set; } = new();
    }
}