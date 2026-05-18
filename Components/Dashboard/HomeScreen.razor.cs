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

        protected int _activeModules;
        protected int _scansCompleted;
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
            _scansCompleted = 0;

            _activities = new List<ActivityItem>
            {
                new() { Text = "Signed in successfully", Color = "var(--success)", Time = "Just now" },
                new() { Text = "Account created on BASTION", Color = "var(--info)", Time = DateTime.Now.ToString("MMM dd") },
                new() { Text = "Welcome to your Digital Fortress!", Color = "var(--home-primary)", Time = DateTime.Now.ToString("MMM dd") }
            };
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender && AuthService.IsAuthenticated && !_gaugeAnimated)
            {
                _gaugeAnimated = true;
                try
                {
                    await JS.InvokeVoidAsync("bastionHome.startClock", "dashboard-clock");
                    await JS.InvokeVoidAsync("bastionHome.animateGauge", "score-gauge-ring", 25);
                    await JS.InvokeVoidAsync("bastionAnimate.countUp", "score-value", 25, 1500);
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
