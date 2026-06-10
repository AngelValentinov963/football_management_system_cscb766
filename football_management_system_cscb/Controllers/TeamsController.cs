
using football_management_system_cscb.Data;
using football_management_system_cscb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class TeamsController : Controller
{
    private readonly FootballDbContext _context;

    public TeamsController(FootballDbContext context)
    {
        _context = context;
    }

    // GET: TEAMS
    public async Task<IActionResult> Index()
    {
        return View(await _context.Teams.ToListAsync());
    }

    // GET: TEAMS/Details/5
    public async Task<IActionResult> Details(int? teamid)
    {
        if (teamid == null)
        {
            return NotFound();
        }

        var team = await _context.Teams
            .FirstOrDefaultAsync(m => m.TeamId == teamid);
        if (team == null)
        {
            return NotFound();
        }

        return View(team);
    }

    // GET: TEAMS/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: TEAMS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("TeamId,Name,City")] Team team)
    {
        if (ModelState.IsValid)
        {
            _context.Add(team);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(team);
    }

    // GET: TEAMS/Edit/5
    public async Task<IActionResult> Edit(int? teamid)
    {
        if (teamid == null)
        {
            return NotFound();
        }

        var team = await _context.Teams.FindAsync(teamid);
        if (team == null)
        {
            return NotFound();
        }
        return View(team);
    }

    // POST: TEAMS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? teamid, [Bind("TeamId,Name,City")] Team team)
    {
        if (teamid != team.TeamId)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(team);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TeamExists(team.TeamId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        return View(team);
    }

    // GET: TEAMS/Delete/5
    public async Task<IActionResult> Delete(int? teamid)
    {
        if (teamid == null)
        {
            return NotFound();
        }

        var team = await _context.Teams
            .FirstOrDefaultAsync(m => m.TeamId == teamid);
        if (team == null)
        {
            return NotFound();
        }

        return View(team);
    }

    // POST: TEAMS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? teamid)
    {
        var team = await _context.Teams.FindAsync(teamid);
        if (team != null)
        {
            _context.Teams.Remove(team);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool TeamExists(int? teamid)
    {
        return _context.Teams.Any(e => e.TeamId == teamid);
    }

    [HttpPost]
    public IActionResult AssignTeam(int teamId)
    {
        var userId = HttpContext.Session.GetInt32("UserId");

        if (userId == null)
            return Unauthorized();

        var user = _context.Users
            .FirstOrDefault(u => u.UserId == userId.Value);

        if (user == null)
            return Unauthorized();
        if (user.TeamId != null)
        {
            return BadRequest("Team already selected.");
        }
        user.TeamId = teamId;

        _context.SaveChanges();

        return Ok();
    }
}
