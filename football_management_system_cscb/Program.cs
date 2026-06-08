using football_management_system_cscb.Data;
using football_management_system_cscb.Service;
using football_management_system_cscb.Services;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);

// ---------------- SERVICES ----------------
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<FootballDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<SeasonService>();
// ⚽ Match engine
builder.Services.AddScoped<TeamService>();
builder.Services.AddScoped<MatchEngine>();

// 🧠 Session support
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(2);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<SessionMatchStore>();

// ---------------- BUILD APP ----------------
var app = builder.Build();

// ---------------- MIDDLEWARE ----------------

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// ⚠️ MUST be BEFORE UseAuthorization + controllers
app.UseSession();

app.UseAuthorization();

// ---------------- ROUTES ----------------
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();