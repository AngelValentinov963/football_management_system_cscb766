using football_management_system_cscb.Data;
using football_management_system_cscb.ViewModels;
using Microsoft.AspNetCore.Mvc;
public class AccountController : Controller
{
    private readonly FootballDbContext _context;

    public AccountController(FootballDbContext context)
    {
        _context = context;
    }

    // GET: Login page
    public IActionResult Login()
    {
        return View();
    }

    // POST: Login form
    [HttpPost]
    public IActionResult Login(LoginViewModel model)
    {
        var user = _context.Users
            .FirstOrDefault(u => u.Username == model.Username
                              && u.Password == model.Password);

        if (user != null)
        {
            // Save logged-in user to session
            HttpContext.Session.SetInt32("UserId", user.UserId);

            return RedirectToAction("Index", "Home");
        }

        ViewBag.Error = "Invalid login";
        return View(model);
    }
}