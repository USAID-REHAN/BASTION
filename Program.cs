using BASTION.Components;
using BASTION.Data;
using BASTION.Interfaces;
using BASTION.Services.Auth;
using BASTION.Services.Dashboard;
using BASTION.Services.Finance;
using BASTION.Services.Gamification;
using BASTION.Services.Security;
using BASTION.Services.Legal;
using BASTION.Services.AI;
using BASTION.Services.Sound;
using BASTION.Services.Theme;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;

// ── QuestPDF Community License ──
QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// ── Authentication ──
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<AdminService>();
builder.Services.AddScoped<AuthenticationStateProvider, BastionAuthStateProvider>();
builder.Services.AddAuthorizationCore();

// ── Core Services ──
builder.Services.AddScoped<ThemeService>();
builder.Services.AddScoped<SoundService>();

// ── Database (SQLite via EF Core) ──
builder.Services.AddDbContext<BastionDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("BastionDb")));

// ── AI Services ──
builder.Services.AddSingleton<GroqService>();
builder.Services.AddSingleton<GroqVisionService>();

// ── FinShield Services (Finance) ──
builder.Services.AddHttpClient<FinShieldService>()
    .ConfigureHttpClient(client =>
    {
        client.DefaultRequestHeaders.Add("User-Agent", "BASTION-FinShield/1.0");
        client.Timeout = TimeSpan.FromSeconds(30);
    });

// ── Gamification Services ──
builder.Services.AddScoped<XpService>();

// ── LexGuard Services ──
builder.Services.AddScoped<DocumentAnalysisService>();
builder.Services.AddScoped<PdfReportService>();

// ── Module Registrations (IModule) ──
builder.Services.AddScoped<IModule, HomescreenModule>();        // Dashboard
builder.Services.AddScoped<IModule, GateKeeperModule>();        // Authentication
builder.Services.AddScoped<IModule, CyberShieldModule>();       // Device Protection
builder.Services.AddScoped<IModule, LexGuardModule>();          // Document Protection
  builder.Services.AddScoped<IModule, FinShieldModule>();       // Financial Fraud Protectionz

builder.Services.AddScoped<IModule, HackerLabModule>();         // Security Academy
builder.Services.AddScoped<IModule, BASTION.Services.Settings.SettingsModule>(); // Personalisation

var app = builder.Build();

// ── Ensure database is created ──
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<BastionDbContext>();
    try
    {
        db.Database.EnsureCreated();

        // Seed Admin User if not exists
        if (!db.Users.Any(u => u.Email == "admin@bastion.app"))
        {
            db.Users.Add(new BASTION.Models.UserAccount
            {
                Email = "admin@bastion.app",
                FullName = "BASTION Admin",
                PasswordHash = Convert.ToBase64String(System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes("Admin@123BASTION_SALT_2026"))),
                Role = "Admin",
                City = "Islamabad",
                CreatedAt = DateTime.UtcNow,
                MfaEnabled = false
            });
            db.SaveChanges();
        }
    }
    catch (Exception ex) when (ex.Message.Contains("no such table", StringComparison.OrdinalIgnoreCase))
    {
        db.Database.EnsureDeleted();
        db.Database.EnsureCreated();

        db.Users.Add(new BASTION.Models.UserAccount
        {
            Email = "admin@bastion.app",
            FullName = "BASTION Admin",
            PasswordHash = Convert.ToBase64String(System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes("Admin@123BASTION_SALT_2026"))),
            Role = "Admin",
            City = "Islamabad",
            CreatedAt = DateTime.UtcNow,
            MfaEnabled = false
        });
        db.SaveChanges();
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
