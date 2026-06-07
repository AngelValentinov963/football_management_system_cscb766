using football_management_system_cscb.Data;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace football_management_system_cscb.Controllers
{
    public class HomeController : Controller
    {
        private readonly FootballDbContext _context;

        public HomeController(FootballDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            ViewBag.TotalPlayers = _context.Players.Count();
            ViewBag.TotalTeams = _context.Teams.Count();
            ViewBag.TotalMatches = _context.FootballMatches.Count();

            ViewBag.TotalGoals = _context.FootballMatches
                .Sum(m => m.HomeScore + m.AwayScore);

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}