using BASTION.Components;
using BASTION.Interfaces;
using BASTION.Services.Auth;
using BASTION.Services.Dashboard;
using BASTION.Services.Finance;
using BASTION.Services.Gamification;
using BASTION.Services.Security;
using BASTION.Services.Legal;
using BASTION.Services.Sound;
using BASTION.Services.Theme;
using Microsoft.AspNetCore.Components.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// ── Authentication ──
builder.Services.AddSingleton<AuthService>();
builder.Services.AddScoped<AuthenticationStateProvider, BastionAuthStateProvider>();
builder.Services.AddAuthorizationCore();

// ── Core Services ──
builder.Services.AddScoped<ThemeService>();
builder.Services.AddScoped<SoundService>();

// ── Module Registrations (IModule) ──
builder.Services.AddScoped<IModule, HomescreenModule>();        // Dashboard
builder.Services.AddScoped<IModule, GateKeeperModule>();        // Authentication
builder.Services.AddScoped<IModule, CyberShieldModule>();       // Device Protection
builder.Services.AddScoped<IModule, LexGuardModule>();          // Document Protection
builder.Services.AddScoped<IModule, FinShieldModule>();         // Financial Fraud Protection
builder.Services.AddScoped<IModule, HackerLabModule>();         // Security Academy
builder.Services.AddScoped<IModule, BASTION.Services.Settings.SettingsModule>(); // Personalisation

var app = builder.Build();

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
