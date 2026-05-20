using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using BASTION.Interfaces;
using BASTION.Services.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BASTION.Components.Dashboard
{
    public partial class HomeScreen : ComponentBase
    {
        [Inject] public IEnumerable<IModule> Modules { get; set; } = default!;
        [Inject] public AuthService AuthService { get; set; } = default!;
        [Inject] public NavigationManager Navigation { get; set; } = default!;
        [Inject] public IJSRuntime JS { get; set; } = default!;
        [Inject] public BASTION.Services.Gamification.XpService XpService { get; set; } = default!;
        [Inject] public BASTION.Data.BastionDbContext DbContext { get; set; } = default!;

        protected int _activeModules;
        protected int _scansCompleted;
        protected int _protectionScore = 20;
        protected string _userRank = "Civilian";
        protected List<ActivityItem> _activities = new();
        protected bool _gaugeAnimated;

        protected override void OnInitialized()
        {
            if (!AuthService.IsAuthenticated)
            {
                Navigation.NavigateTo("/login");
                return;
            }

            _activeModules = Modules.Count(m => m.Name != "GateKeeper" && m.Name != "Dashboard");
            
            var userEmail = AuthService.CurrentUser?.Email ?? "";
            _scansCompleted =
                DbContext.UrlScanLogs.Count(l => l.Email == userEmail) +
                DbContext.PaymentPageAnalyses.Count(l => l.Email == userEmail) +
                DbContext.SecurityScanLogs.Count(l => l.Email == userEmail) +
                DbContext.LexGuardAnalyses.Count(l => l.UserId == userEmail);

            _protectionScore = 20;
            if (AuthService.CurrentUser != null)
            {
                var currentXp = DbContext.Users
                    .Where(u => u.Email == userEmail)
                    .Select(u => u.CurrentXp)
                    .FirstOrDefault();
                _protectionScore += Math.Min(50, currentXp / 10);
                if (AuthService.CurrentUser.MfaEnabled) _protectionScore += 30;
            }

            var dbRank = DbContext.Users
                .Where(u => u.Email == userEmail)
                .Select(u => u.CurrentRank)
                .FirstOrDefault();
            _userRank = AuthService.IsAdmin ? "Admin" : dbRank ?? XpService.CurrentRank;

            var logs = DbContext.AuditLogs
                .Where(l => l.Email == userEmail)
                .OrderByDescending(l => l.Timestamp)
                .Take(10)
                .ToList();

            _activities = logs.Select(l => new ActivityItem
            {
                Text = l.Action,
                Color = l.Action.Contains("Scan") || l.Action.Contains("Audit") ? "var(--success)" :
                        l.Action.Contains("Login") ? "var(--info)" : "var(--home-primary)",
                Time = l.Timestamp.ToString("MMM dd")
            }).ToList();

            if (_activities.Count == 0)
            {
                _activities.Add(new ActivityItem { Text = "Welcome to your Digital Fortress!", Color = "var(--home-primary)", Time = "Just now" });
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender && AuthService.IsAuthenticated && !_gaugeAnimated)
            {
                _gaugeAnimated = true;
                StateHasChanged(); // Trigger re-render for CSS transition
                try
                {
                    await JS.InvokeVoidAsync("bastionHome.startClock", "dashboard-clock");
                    await JS.InvokeVoidAsync("bastionAnimate.countUp", "score-value", _protectionScore, 1500);
                }
                catch { /* JS not ready */ }
            }
        }

        protected string GetDelayClass(int order) => order switch
        {
            <= 3 => "delay-1",
            <= 4 => "delay-2",
            <= 5 => "delay-3",
            <= 6 => "delay-4",
            _ => "delay-5"
        };

        protected class ActivityItem
        {
            public string Text { get; set; } = "";
            public string Color { get; set; } = "";
            public string Time { get; set; } = "";
        }
    }
}
