using football_management_system_cscb.Data;
using football_management_system_cscb.ViewModel;
using football_management_system_cscb.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login", "Account");

            var user = _context.Users
                .FirstOrDefault(u => u.UserId == userId.Value);

            if (user == null)
                return RedirectToAction("Login", "Account");

            if (user.TeamId == null)
                return RedirectToAction("Index", "Teams");

            var players = _context.Players
                .Where(p => p.TeamId == user.TeamId.Value)
                .Select(p => new PlayerViewModel
                {
                    PlayerId = p.PlayerId,
                    FullName = p.FirstName + " " + p.LastName,
                    PreferredPosition = p.PreferredPosition,
                    OverallRating = p.OverallRating
                })
                .ToList();

            var team = _context.Teams.FirstOrDefault(t => t.TeamId == user.TeamId.Value);

            var model = new SquadViewModel
            {
                TeamId = user.TeamId.Value,
                Squad = players,
                StartingXI = new List<PlayerViewModel>(),
                Formation = team?.DefaultFormation ?? "4-3-3"
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SaveFormation([FromBody] string formation)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return Unauthorized();

            var user = await _context.Users.FindAsync(userId.Value);
            if (user?.TeamId == null) return BadRequest();

            var team = await _context.Teams.FindAsync(user.TeamId.Value);
            if (team == null) return NotFound();

            team.DefaultFormation = formation;
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}