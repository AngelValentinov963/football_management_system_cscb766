using football_management_system_cscb.Data;
using football_management_system_cscb.Models;
using football_management_system_cscb.Service;
using football_management_system_cscb.ViewModel;
using football_management_system_cscb.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class MatchController : Controller
{
    private readonly MatchEngine _engine;
    private readonly FootballDbContext _db;
    private readonly SessionMatchStore _session;

    public MatchController(
        MatchEngine engine,
        FootballDbContext db,
        SessionMatchStore session)
    {
        _engine = engine;
        _db = db;
        _session = session;
    }

    // -------------------------
    // PAGE
    // -------------------------
    public async Task<IActionResult> Index()
    {
        var state = _session.Load();

        if (state == null)
            return View();

        ViewBag.HomeTeam = await _db.Teams
            .FirstOrDefaultAsync(t => t.TeamId == state.HomeTeamId);

        ViewBag.AwayTeam = await _db.Teams
            .FirstOrDefaultAsync(t => t.TeamId == state.AwayTeamId);

        return View();
    }

    // -------------------------
    // START MATCH
    // -------------------------
    public async Task<IActionResult> Start(int homeId, int awayId)
    {
        var home = await _db.Teams.Include(t => t.Players)
            .FirstOrDefaultAsync(t => t.TeamId == homeId);

        var away = await _db.Teams.Include(t => t.Players)
            .FirstOrDefaultAsync(t => t.TeamId == awayId);

        if (home == null || away == null)
            return NotFound("Teams not found");

        var homeSquad = new Squad(home);
        var awaySquad = new Squad(away);

        homeSquad.AutoSelect();
        awaySquad.AutoSelect();

        var state = _engine.StartMatch();

        // store match metadata
        state.HomeTeamId = homeId;
        state.AwayTeamId = awayId;

        _session.Save(state);

        HttpContext.Session.SetInt32("homeId", homeId);
        HttpContext.Session.SetInt32("awayId", awayId);

        return RedirectToAction("Index");
    }

    // -------------------------
    // TICK (called every second)
    // -------------------------
    [HttpPost]
    public async Task<IActionResult> Tick()
    {
        var state = _session.Load();

        if (state == null)
            return BadRequest("No match running");

        var homeId = HttpContext.Session.GetInt32("homeId");
        var awayId = HttpContext.Session.GetInt32("awayId");

        if (homeId == null || awayId == null)
            return BadRequest("No match running");

        var home = await _db.Teams.Include(t => t.Players)
            .FirstAsync(t => t.TeamId == homeId);

        var away = await _db.Teams.Include(t => t.Players)
            .FirstAsync(t => t.TeamId == awayId);

        var homeSquad = new Squad(home);
        var awaySquad = new Squad(away);

        homeSquad.AutoSelect();
        awaySquad.AutoSelect();

        _engine.AdvanceMinute(state, homeSquad, awaySquad);

        _session.Save(state);

        return Json(state);
    }

    // -------------------------
    // GET CURRENT STATE
    // -------------------------
    [HttpGet]
    public IActionResult State()
    {
        return Json(_session.Load());
    }
}