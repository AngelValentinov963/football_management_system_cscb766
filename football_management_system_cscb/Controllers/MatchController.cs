using football_management_system_cscb.Data;
using football_management_system_cscb.Models;
using football_management_system_cscb.Models.Formation;
using football_management_system_cscb.Service;
using football_management_system_cscb.Services;
using football_management_system_cscb.ViewModel;
using football_management_system_cscb.ViewModels;
using football_management_system_cscb.ViewModels.Season;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class MatchController : Controller
{
    private readonly MatchEngine _engine;
    private readonly FootballDbContext _db;
    private readonly SessionMatchStore _session;
    private readonly SquadService _squadService;

    public MatchController(
        MatchEngine engine,
        FootballDbContext db,
        SessionMatchStore session,
         SquadService squadService)
    {
        _engine = engine;
        _db = db;
        _session = session;
        _squadService = squadService; // ✅ ASSIGN IT

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
    public async Task<IActionResult> Start(int homeId, int awayId, int fixtureId)
    {
        var home = await _db.Teams
            .Include(t => t.Players)
            .FirstOrDefaultAsync(t => t.TeamId == homeId);

        var away = await _db.Teams
            .Include(t => t.Players)
            .FirstOrDefaultAsync(t => t.TeamId == awayId);

        if (home == null || away == null)
            return NotFound("Teams not found");

        var homeFormation = FormationLibrary.Get(home.DefaultFormation);
        var awayFormation = FormationLibrary.Get(away.DefaultFormation);

        var homeSquad = _squadService.GetSquad(homeId, homeFormation);
        var awaySquad = _squadService.GetSquad(awayId, awayFormation);

        homeSquad.BuildSquad(home.Players.ToList());
        awaySquad.BuildSquad(away.Players.ToList());

        var state = _engine.StartMatch();

        state.HomeTeamId = homeId;
        state.AwayTeamId = awayId;

        state.HomeSquad = homeSquad;
        state.AwaySquad = awaySquad;

        _session.Save(state);

        HttpContext.Session.SetInt32("homeId", homeId);
        HttpContext.Session.SetInt32("awayId", awayId);
        HttpContext.Session.SetInt32("fixtureId", fixtureId);

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

        var homeFormation = FormationLibrary.Get(home.DefaultFormation);
        var awayFormation = FormationLibrary.Get(away.DefaultFormation);

        var homeSquad = _squadService.GetSquad(homeId, homeFormation);
        var awaySquad = _squadService.GetSquad(awayId, awayFormation);

        _engine.AdvanceMinute(state, homeSquad, awaySquad);

        _session.Save(state);

        // 🟢 HERE is where you use it
        if (state.CurrentMinute >= 90 && !state.IsPaused)
        {
            var fixtureId = HttpContext.Session.GetInt32("fixtureId");

            if (fixtureId != null)
            {
                await SaveMatchResult(state, fixtureId);
            }

            state.IsFinished = true;
            _session.Save(state);
        }

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

    private async Task SaveMatchResult(MatchState state,int? fixtureId)
    {
       var fixture = await _db.Fixtures
            .FirstOrDefaultAsync(f => f.Id == fixtureId);
       
       if (fixture == null)
           throw new Exception("Fixture not found");
       
       fixture.HomeGoals = state.HomeGoals;
       fixture.AwayGoals = state.AwayGoals;
       fixture.Played = true;

        await _db.SaveChangesAsync();
    }
}