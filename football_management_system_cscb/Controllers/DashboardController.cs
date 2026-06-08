using Microsoft.AspNetCore.Mvc;

public class DashboardController : Controller
{
    private readonly TeamFormService _teamFormService;

    public DashboardController(TeamFormService teamFormService)
    {
        _teamFormService = teamFormService;
    }

    public IActionResult Dashboard()
    {
        var form = _teamFormService.GetTeamsForm(5);

        ViewBag.LeagueTable = _teamFormService.GetLeagueTable();

        return View(form);
    }
}