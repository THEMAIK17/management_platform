using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Platform.Application;
using Platform.Infrastructure;
using Platform.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// ── Database ──────────────────────────────────────────────────
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// ── Application & Infrastructure Services ────────────────────
builder.Services.AddApplication();
builder.Services.AddInfrastructure();

// ── Authentication ───────────────────────────────────────────
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
    });

// ── MVC ───────────────────────────────────────────────────────
builder.Services.AddControllersWithViews();

var app = builder.Build();

// ── Professional Startup Message ──────────────────────────────
var displayUrl = app.Configuration["DISPLAY_URL"] ?? "http://localhost:5229";
Console.WriteLine(" \n" +
                  "  ╔══════════════════════════════════════════════════════════════╗\n" +
                  "  ║                                                              ║\n" +
                  "  ║   MANAGEMENT PLATFORM - WEB INTERFACE                        ║\n" +
                  "  ║   The service is starting up...                              ║\n" +
                  "  ║                                                              ║\n" +
                  "  ║   Access the platform at:                                    ║\n" +
                  $"  ║   {displayUrl,-59}║\n" +
                  "  ║                                                              ║\n" +
                  "  ╚══════════════════════════════════════════════════════════════╝\n");

// ── Middleware ────────────────────────────────────────────────
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication(); // Added
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();