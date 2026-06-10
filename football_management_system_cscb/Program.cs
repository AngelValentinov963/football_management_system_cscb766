using football_management_system_cscb.Data;
using football_management_system_cscb.Service;
using football_management_system_cscb.Services;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);

// ---------------- SERVICES ----------------
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<PlayerValueService>();

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
builder.Services.AddScoped<TeamFormService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<SessionMatchStore>();
builder.Services.AddScoped<SquadService>();
builder.Services.AddScoped<FormationMatchupService>();
builder.Services.AddScoped<MatchEngine>();
builder.Services.AddScoped<SeasonService>();

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