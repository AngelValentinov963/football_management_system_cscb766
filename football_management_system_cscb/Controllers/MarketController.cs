    using football_management_system_cscb.Data;
    using football_management_system_cscb.Services;
    using football_management_system_cscb.ViewModel;
    using football_management_system_cscb.ViewModels;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;

    namespace football_management_system_cscb.Controllers
    {
        public class MarketController : Controller
        {
            private readonly FootballDbContext _context;
            private readonly PlayerValueService _valueService;

            public MarketController(FootballDbContext context, PlayerValueService valueService)
            {
                _context = context;
                _valueService = valueService;
            }

            public IActionResult Index()
            {
                var userId = HttpContext.Session.GetInt32("UserId");

                if (userId == null)
                    return RedirectToAction("Login", "Account");

                var user = _context.Users
                    .FirstOrDefault(x => x.UserId == userId.Value);

                if (user?.TeamId == null)
                    return RedirectToAction("Index", "Teams");

                var myTeamId = user.TeamId.Value;

            var model = new MarketViewModel
            {
                Team = _context.Teams.First(t => t.TeamId == myTeamId),

                MyPlayers = _context.Players
.Where(p => p.TeamId == myTeamId)
.ToList()
.Select(p => new PlayerViewModel
{
    PlayerId = p.PlayerId,
    FullName = p.FirstName + " " + p.LastName,
    PreferredPosition = p.PreferredPosition,
    Nationality = p.Nationality,
    BirthDate = p.BirthDate,
    OverallRating = p.OverallRating ?? 0,
    IsListedForTransfer = p.IsListedForTransfer,
    MarketValue = _valueService.CalculatePlayerValue(p)
})
.ToList(),

                AvailablePlayers = _context.Players
.Where(p => p.TeamId != myTeamId && p.IsListedForTransfer)
.ToList()
.Select(p => new PlayerViewModel
{
    PlayerId = p.PlayerId,
    FullName = p.FirstName + " " + p.LastName,
    PreferredPosition = p.PreferredPosition,
    Nationality = p.Nationality,
    BirthDate = p.BirthDate,
    OverallRating = p.OverallRating ?? 0,
    IsListedForTransfer = p.IsListedForTransfer,
    MarketValue = _valueService.CalculatePlayerValue(p)
})
.ToList()
            };

                return View(model);
            }

            [HttpPost]
            public IActionResult Buy(int playerId)
            {
                var userId = HttpContext.Session.GetInt32("UserId");

                if (userId == null)
                    return RedirectToAction("Login", "Account");

                var user = _context.Users
                    .First(u => u.UserId == userId.Value);

                if (user?.TeamId == null)
                    return RedirectToAction("Index", "Teams");

                var myTeam = _context.Teams
                    .First(t => t.TeamId == user.TeamId);

                var player = _context.Players
                    .First(p => p.PlayerId == playerId);

                var sellerTeam = player.TeamId != null
                    ? _context.Teams.FirstOrDefault(t => t.TeamId == player.TeamId)
                    : null;

                var value = _valueService.CalculatePlayerValue(player);

                if (myTeam.Budget < value)
                {
                    TempData["Error"] = "Not enough budget.";
                    return RedirectToAction(nameof(Index));
                }

                myTeam.Budget -= value;

                if (sellerTeam != null)
                    sellerTeam.Budget += value;

                player.TeamId = myTeam.TeamId;
                player.IsListedForTransfer = false;

                _context.SaveChanges();

                TempData["Success"] = $"{player.FirstName} signed!";

                return RedirectToAction(nameof(Index));
            }

        [HttpPost]
        public IActionResult Sell(int playerId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var user = _context.Users.FirstOrDefault(u => u.UserId == userId.Value);
            if (user?.TeamId == null)
                return RedirectToAction("Index", "Teams");

            var player = _context.Players
                .FirstOrDefault(p => p.PlayerId == playerId && p.TeamId == user.TeamId);

            if (player == null)
            {
                TempData["Error"] = "Player not found in your squad.";
                return RedirectToAction(nameof(Index));
            }

            player.IsListedForTransfer = true;
            _context.SaveChanges();

            TempData["Success"] = $"{player.FirstName} listed for transfer!";
            return RedirectToAction(nameof(Index));
        }
    }
    }