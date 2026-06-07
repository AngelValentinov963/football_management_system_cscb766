using football_management_system_cscb.ViewModels;
using football_management_system_cscb.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace football_management_system_cscb.Controllers
{
    public class PlayerController : Controller
    {
        private readonly FootballDbContext _db;

        public PlayerController(FootballDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            var players = _db.Players
                .Select(p => new PlayerViewModel
                {
                    PlayerId = p.PlayerId,
                    FullName = p.FirstName + " " + p.LastName,
                    BirthDate = p.BirthDate,
                    Nationality = p.Nationality,
                    OverallRating = p.OverallRating,
                    Potential = p.Potential,
                    PreferredPosition = p.PreferredPosition,
                    MarketValue = p.MarketValue
                })
                .ToList();

            return View(players);
        }
    }
}