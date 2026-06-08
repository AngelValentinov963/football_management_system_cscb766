using football_management_system_cscb.Data;
using football_management_system_cscb.ViewModel;
using football_management_system_cscb.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace football_management_system_cscb.Controllers
{
    public class SquadController : Controller
    {
        private readonly FootballDbContext _context;

        public SquadController(FootballDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var players = _context.Players
                .Select(p => new PlayerViewModel
                {
                    PlayerId = p.PlayerId,
                    FullName = p.FirstName + p.LastName,
                    PreferredPosition = p.PreferredPosition,
                    OverallRating = p.OverallRating
                })
                .ToList();

            var model = new SquadViewModel
            {
                Squad = players,
                StartingXI = new List<PlayerViewModel>(),
                Formation = "4-3-3"
            };

            return View(model);
        }
    }
}
