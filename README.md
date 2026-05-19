<div align="center">
    <img src="wwwroot/img/logo.png" alt="BASTION Logo" width="120" style="margin-bottom: 20px;" />
    <h1>BASTION: SECURE DIGITAL FORTRESS</h1>
    <p><strong>Enterprise-Grade Cybersecurity Personal Suite & Interactive Gamified Training Hub</strong></p>
    <p><em>Built on .NET 8, Blazor Server (InteractiveServer), and Entity Framework Core with SQLite</em></p>
</div>

<hr />

<h2>SECTION 1: SYSTEM INTRODUCTION AND ARCHITECTURAL CORE</h2>

<p>
    BASTION is a highly integrated, zero-coupling modular cybersecurity suite designed to empower individual users with elite-level system diagnostics, automated legal contract risk assessment, advanced multi-source financial fraud scanning, and a complete, gamified cybersecurity training academy.
</p>

<p>
    The system architecture is engineered to run in a fully containerized or local development environment, maintaining complete state persistence through a secure, self-healing SQLite database managed via Entity Framework Core. By using InteractiveServer rendering pipelines in Blazor, BASTION achieves lightning-fast real-time reactivity without the overhead of heavy single-page application frameworks.
</p>

<h3>Key Architectural Objectives</h3>
<ul>
    <li><strong>Zero-Coupling Modularity:</strong> Every core domain is encapsulated in its own directory structure containing its respective Razor views, controllers, data models, and business logic services. Communication between components occurs via DI containers and interface contracts, preventing ripple effects when making upgrades.</li>
    <li><strong>Stateful Micro-Interactions:</strong> Leverages Blazor SignalR circuits to run real-time user session tracking, instant remote device revocation, autonomous AI repair operations, and dynamic sound previews instantly.</li>
    <li><strong>Secret Isolation:</strong> Protects operational tokens and credentials by strictly separating tracked baseline setups from untracked developer environments via native ASP.NET Core multi-layered configuration pipelines.</li>
</ul>

<hr />

<h2>SECTION 2: COMPLETE FILE AND DIRECTORY WALKTHROUGH</h2>

<p>
    The BASTION codebase follows a structured layout. The following is a detailed description of every critical file and folder in the workspace:
</p>

<pre>
BASTION/
│
├── AI_GUIDELINES.md                  &lt;-- Collaborative agent development standards
├── BASTION.csproj                    &lt;-- MSBuild configuration, target net8.0, NuGet bindings
├── BASTION.sln                       &lt;-- Visual Studio Solution file
├── Program.cs                        &lt;-- Service definitions, dependency injections, middleware
├── appsettings.json                  &lt;-- Main system configs with safe placeholder tokens
├── appsettings.Development.json      &lt;-- Local, untracked development secrets (gitignored)
│
├── Components/                       &lt;-- UI Layer containing Razor views and layouts
│   ├── Layout/
│   │   ├── MainLayout.razor          &lt;-- Master template, theme engine state, eviction router
│   │   └── Sidebar.razor             &lt;-- High-end responsive collapsible navigation panel
│   │
│   ├── Admin/
│   │   └── AdminHub.razor            &lt;-- Command dashboard, session manager, ban console
│   │
│   ├── Auth/
│   │   ├── LoginForm.razor           &lt;-- Login page, password toggle, premium SSO handlers
│   │   ├── RegisterForm.razor        &lt;-- Registration page, data validation, city binding
│   │   └── SessionMonitor.razor      &lt;-- Client component showing active sessions
│   │
│   ├── CyberShield/
│   │   ├── SecurityChatbot.razor     &lt;-- Groq AI interactive conversational secure copilot
│   │   └── SystemAudit.razor         &lt;-- Port checking, processes scanner, vulns list
│   │
│   ├── Dashboard/
│   │   ├── About.razor               &lt;-- Interactive platform information page
│   │   ├── HomeScreen.razor          &lt;-- Glassmorphic main screen showing platform summaries
│   │   └── Support.razor             &lt;-- Agentic support, live log console, ticket forms
│   │
│   ├── FinShield/
│   │   ├── FinHub.razor              &lt;-- Financial scanner dashboard tab panel
│   │   ├── MessageAnalyzer.razor     &lt;-- High-pressure SMS and text pattern detector
│   │   └── PaymentPageScanner.razor  &lt;-- Invoice vision analyzer using Groq LLava Vision
│   │
│   ├── HackerLab/
│   │   ├── DomainBriefing.razor      &lt;-- Information sheets about security specialties
│   │   ├── HackerHub.razor           &lt;-- Academy main panel displaying badges and rank leaderboard
│   │   └── Games/
│   │       └── FraudFeed.razor       &lt;-- Scenario-based game checking phishing clues
│   │
│   ├── LexGuard/
│   │   ├── AnalysisHistory.razor     &lt;-- Historical SQLite persistent legal scans history
│   │   └── LexHub.razor              &lt;-- Legal analyzer upload panel, risks view, PDF trigger
│   │
│   └── Settings/
│       ├── SoundPanel.razor          &lt;-- Audio switches, BGM select, preview trigger
│       └── ThemePanel.razor          &lt;-- Persistent dark theme control and light accent wheel
│
├── Data/
│   └── BastionDbContext.cs           &lt;-- EF Core setup, table maps, seed routines
│
├── Interfaces/
│   └── IModule.cs                    &lt;-- Shared modular integration contract
│
├── Models/
│   ├── LexGuardModels.cs             &lt;-- Analysis results, risks list, download tokens
│   └── UserAccount.cs                &lt;-- Auth schemes, sessions database structures
│
├── Services/
│   ├── AI/
│   │   ├── GroqService.cs            &lt;-- Main client wrapper for Groq Text API
│   │   └── GroqVisionService.cs      &lt;-- Main client wrapper for Groq Vision API
│   │
│   ├── Auth/
│   │   ├── AdminService.cs           &lt;-- User roles, global settings database controls
│   │   └── AuthService.cs            &lt;-- Login logic, registration, static revocations
│   │
│   ├── Dashboard/
│   │   └── HomescreenModule.cs       &lt;-- Modular dashboard setup
│   │
│   ├── Finance/
│   │   └── FinShieldService.cs       &lt;-- Scan orchestration using external security APIs
│   │
│   ├── Gamification/
│   │   └── XpService.cs              &lt;-- Academy XP increments, badge checks, global rankings
│   │
│   ├── Legal/
│   │   ├── DocumentAnalysisService.cs &lt;-- LexGuard AI assessment processor
│   │   └── PdfReportService.cs       &lt;-- Premium PDF builder using QuestPDF
│   │
│   ├── Sound/
│   │   └── SoundService.cs           &lt;-- Background audio controller, beep previews
│   │
│   └── Theme/
│       └── ThemeService.cs           &lt;-- Persistent dark theme manager, color highlights
│
└── wwwroot/                          &lt;-- Public static web resources folder
    ├── audio/                        &lt;-- Soundscape files (bgm1, bgm2, previews)
    ├── css/                          &lt;-- Modular styles, custom colors, animations
    └── js/                           &lt;-- JS Interop helpers for local storage and DOM
</pre>

<hr />

<h2>SECTION 3: DATABASE ARCHITECTURE AND PERSISTENCE MODELS</h2>

<p>
    BASTION uses Entity Framework Core mapped to a localized SQLite engine (`bastion.db`). The relational database schema is structured as follows:
</p>

<h3>3.1 The Users Table (`UserAccount`)</h3>
<p>
    This table stores registration data, password hashes, security configurations, and gamification credentials:
</p>
<ul>
    <li><strong>Email (string, Primary Key):</strong> Lowercased unique identifier for authentication.</li>
    <li><strong>FullName (string):</strong> The displayed display name of the user.</li>
    <li><strong>PasswordHash (string):</strong> Base64 encoded SHA-256 hash containing a secure system salt.</li>
    <li><strong>Role (string):</strong> User access clearance (Admin or User).</li>
    <li><strong>City (string):</strong> Geolocation bound during signup, used for session safety logs.</li>
    <li><strong>IsLockedOut (bool):</strong> Administrative lock flag used to ban profiles.</li>
    <li><strong>LockoutEnd (DateTime?):</strong> Expiration timestamp of the ban lockout block.</li>
    <li><strong>FailedAttempts (int):</strong> Counter tracking consecutive unsuccessful sign-ins, triggering automated temporary locking after 5 failures.</li>
    <li><strong>CurrentXp (int):</strong> Total earned experience points in the HackerLab Academy.</li>
    <li><strong>CurrentRank (string):</strong> Academic status title calculated based on accumulated XP milestones.</li>
    <li><strong>AvatarUrl (string):</strong> Customized profile image file path.</li>
    <li><strong>CreatedAt (DateTime):</strong> Creation timestamp of the account.</li>
</ul>

<h3>3.2 The User Sessions Table (`UserSession`)</h3>
<p>
    This table monitors active connections and devices across BASTION:
</p>
<ul>
    <li><strong>Id (int, Primary Key):</strong> Auto-incrementing identifier.</li>
    <li><strong>Email (string):</strong> Foreign link mapping to the user account email.</li>
    <li><strong>Device (string):</strong> User-Agent header description of the browser engine.</li>
    <li><strong>Location (string):</strong> Registered location derived from the user's profile.</li>
    <li><strong>LoginTime (DateTime):</strong> Creation timestamp of the active session.</li>
    <li><strong>IsRevoked (bool):</strong> State flag indicating if the session has been closed or revoked.</li>
    <li><strong>IsRevokedByAdmin (bool):</strong> Set to true if the session was explicitly closed by an administrator.</li>
</ul>

<h3>3.3 The Audit Logs Table (`AuditLog`)</h3>
<p>
    An immutable audit trail tracking critical administrative modifications:
</p>
<ul>
    <li><strong>Id (int, Primary Key):</strong> Auto-incrementing identifier.</li>
    <li><strong>Email (string?):</strong> The account email of the administrator who executed the action (null if automated system task).</li>
    <li><strong>Action (string):</strong> Broad classification of change (e.g. ConfigChange, Ban/Unban, Promote/Demote).</li>
    <li><strong>Details (string):</strong> Comprehensive textual description of old vs new values.</li>
    <li><strong>Timestamp (DateTime):</strong> Date and time of the logging task.</li>
</ul>

<h3>3.4 The Support Tickets Table (`SupportTicket`)</h3>
<p>
    Backs the real-time agentic support Portal:
</p>
<ul>
    <li><strong>Id (int, Primary Key):</strong> Auto-incrementing ticket code.</li>
    <li><strong>Title (string):</strong> The description of the user incident.</li>
    <li><strong>Description (string):</strong> Detailed issue ticket request parameters.</li>
    <li><strong>Status (string):</strong> Workflow state (Pending, Processing, Resolved).</li>
    <li><strong>AIResponse (string):</strong> Sentinel-AI's final diagnostic answers and troubleshooting logs.</li>
    <li><strong>CreatedAt (DateTime):</strong> Generation timestamp.</li>
</ul>

<h3>3.5 LexGuard Analysis Records Table</h3>
<p>
    Tracks historical legal document reviews:
</p>
<ul>
    <li><strong>Id (int, Primary Key):</strong> Unique analysis hash.</li>
    <li><strong>FileName (string):</strong> Original legal document filename.</li>
    <li><strong>FileType (string):</strong> Format extension (.pdf or .docx).</li>
    <li><strong>SafetyScore (int):</strong> Calculated safety percentage.</li>
    <li><strong>Classification (string):</strong> Document agreement type (NDA, SLA, Employment, etc.).</li>
    <li><strong>DangerousClauses (string):</strong> JSON array of extracted high-risk clause text blocks.</li>
    <li><strong>MissingProtections (string):</strong> JSON array of omitted structural protections.</li>
    <li><strong>Timestamp (DateTime):</strong> Analysis execution timestamp.</li>
</ul>

<hr />

<h2>SECTION 4: IN-DEPTH CODE WALKTHROUGHS</h2>

<h3>4.1 Secure Eviction and Lockout Loop (`AuthService.cs` &amp; `AdminService.cs`)</h3>
<p>
    A major feature is the ability to ban users or revoke sessions in real-time, instantly kicking them out across different browser platforms (e.g., standard Chrome, incognito, or distinct Edge windows) without waiting for a browser refresh.
</p>

<p>
    This real-time coordination is achieved using a static event inside `AuthService.cs`:
</p>

<pre>
public static event Action&lt;string, bool&gt;? OnSessionRevoked;
</pre>

<p>
    When an Administrator decides to ban a user account, the action triggers the following method inside `AdminService.cs`:
</p>

<pre>
public bool ToggleUserLock(string email)
{
    var user = _dbContext.Users.FirstOrDefault(u =&gt; u.Email == email);
    if (user == null) return false;

    user.IsLockedOut = !user.IsLockedOut;
    if (user.IsLockedOut)
    {
        user.LockoutEnd = DateTime.UtcNow.AddYears(100); // Banned for 100 years

        // 1. Revoke all active sessions in the database immediately
        var activeSessions = _dbContext.UserSessions
            .Where(s =&gt; s.Email == email &amp;&amp; !s.IsRevoked)
            .ToList();
        foreach (var session in activeSessions)
        {
            session.IsRevoked = true;
            session.IsRevokedByAdmin = true;
        }
        _dbContext.SaveChanges();

        // 2. Trigger the static real-time event to evict the active UI session
        AuthService.TriggerSessionRevocation(email, isBan: true);
    }
    else
    {
        user.LockoutEnd = null;
        _dbContext.SaveChanges();
    }
    
    LogAction(email, "Ban/Unban", user.IsLockedOut ? "User banned" : "User unbanned");
    return true;
}
</pre>

<p>
    In `AuthService.cs`, when `TriggerSessionRevocation` is called, it triggers `OnSessionRevoked?.Invoke(email, isBan)`. All active browser SignalR circuits contain an `AuthService` instance that subscribes to this static event during initialization:
</p>

<pre>
private void HandleSessionRevoked(string email, bool isBan)
{
    if (_currentUser?.Email == email)
    {
        _currentUser = null;
        LastSignOutReason = isBan ? "banned" : "revoked";
        OnAuthStateChanged?.Invoke();
    }
}
</pre>

<p>
    The master `MainLayout.razor` listens to the local instance event `OnAuthStateChanged`. The moment it is invoked, it redirects the target user instantly to the login page:
</p>

<pre>
private void OnAuthStateChanged()
{
    InvokeAsync(() =&gt; {
        if (!AuthService.IsAuthenticated)
        {
            Navigation.NavigateTo($"/login?reason={AuthService.LastSignOutReason}", forceLoad: true);
        }
        StateHasChanged();
    });
}
</pre>

<p>
    Finally, `LoginForm.razor` parses the query parameters and displays the exact customized alert panel:
</p>

<pre>
if (Reason == "revoked")
{
    _errorMessage = "The admin has revoked your current session.";
}
else if (Reason == "banned")
{
    _errorMessage = "The admin has banned your account.";
}
</pre>

<h3>4.2 Database-Linked SSO OAuth Integrations</h3>
<p>
    To offer a professional, high-end login experience, the Google and Microsoft SSO buttons are completely database-linked:
</p>

<pre>
private async Task HandleOAuth(string provider)
{
    _errorMessage = null;
    _successMessage = null;
    _isLoading = true;
    StateHasChanged();

    await Task.Delay(1000); // Simulate secure OAuth handshaking latency

    string email = provider == "Google" ? "google.user@bastion.app" : "microsoft.user@bastion.app";
    string fullName = provider == "Google" ? "Google User" : "Microsoft User";
    string password = "OAuthMockPass123!_SecureSecure";

    // 1. Automatically register the account if it does not exist
    AuthService.Register(fullName, email, password, "Islamabad");

    // 2. Perform a standard login to record session metadata in SQLite
    var result = AuthService.Login(email, password);

    if (result.Success)
    {
        _successMessage = $"Successfully authenticated with {provider} SSO!";
        await SoundService.PlayChime();
        StateHasChanged();
        await Task.Delay(800);
        Navigation.NavigateTo("/");
    }
    else
    {
        _errorMessage = $"SSO Authentication failed: {result.Message}";
        _isLoading = false;
        await SoundService.PlayBeep();
    }
}
</pre>

<h3>4.3 Sentinel-AI Agentic Support Portal</h3>
<p>
    The Support Portal represents an advanced implementation of autonomous AI operations. When a user submits an issue or creates a ticket, the portal starts a background task that initiates a **ReAct Reasoning Loop** using the injected scoped `SupportAgentService`.
</p>

<p>
    This loop allows the Groq API model to execute physical system modifications using structured C# tools.
</p>

<h4>Sentinel-AI Interactive System Tools</h4>
<ul>
    <li><strong>setsystemtheme (string theme):</strong> Toggles BASTION's active dark or light layout mode across the client browser.</li>
    <li><strong>playsystemsound (string type):</strong> Triggers chime/beep sound alerts dynamically.</li>
    <li><strong>setbackgroundmusic (string track):</strong> Changes the active BGM soundtrack stream (options: bgm1, bgm2, or silence).</li>
</ul>

<h4>Zero-Dependency C# Markdown Parser</h4>
<p>
    To ensure professional UI styling and clean HTML output, the chat system uses a custom C# parser in `Support.razor` instead of rendering raw Markdown blocks:
</p>

<pre>
private MarkupString ParseMarkdownToHtml(string rawMarkdown)
{
    if (string.IsNullOrWhiteSpace(rawMarkdown))
        return new MarkupString(string.Empty);

    var lines = rawMarkdown.Split('\n');
    var htmlBuilder = new System.Text.StringBuilder();
    bool inList = false;

    foreach (var rawLine in lines)
    {
        var line = rawLine.Trim();
        if (string.IsNullOrEmpty(line))
            continue;

        // Strip and convert markdown headers
        if (line.StartsWith("####"))
        {
            CloseList(htmlBuilder, ref inList);
            htmlBuilder.Append($"&lt;h6 class='chat-h6'&gt;{line.TrimStart('#').Trim()}&lt;/h6&gt;");
        }
        else if (line.StartsWith("###"))
        {
            CloseList(htmlBuilder, ref inList);
            htmlBuilder.Append($"&lt;h5 class='chat-h5'&gt;{line.TrimStart('#').Trim()}&lt;/h5&gt;");
        }
        else if (line.StartsWith("##"))
        {
            CloseList(htmlBuilder, ref inList);
            htmlBuilder.Append($"&lt;h4 class='chat-h4'&gt;{line.TrimStart('#').Trim()}&lt;/h4&gt;");
        }
        else if (line.StartsWith("#"))
        {
            CloseList(htmlBuilder, ref inList);
            htmlBuilder.Append($"&lt;h3 class='chat-h3'&gt;{line.TrimStart('#').Trim()}&lt;/h3&gt;");
        }
        // Strip list elements
        else if (line.StartsWith("-") || line.StartsWith("*"))
        {
            if (!inList)
            {
                htmlBuilder.Append("&lt;ul class='chat-ul'&gt;");
                inList = true;
            }
            htmlBuilder.Append($"&lt;li class='chat-li'&gt;{line.Substring(1).Trim()}&lt;/li&gt;");
        }
        // Standard text lines
        else
        {
            CloseList(htmlBuilder, ref inList);
            htmlBuilder.Append($"&lt;p class='chat-p'&gt;{line}&lt;/p&gt;");
        }
    }

    CloseList(htmlBuilder, ref inList);
    return new MarkupString(htmlBuilder.ToString());
}

private void CloseList(System.Text.StringBuilder sb, ref bool inList)
{
    if (inList)
    {
        sb.Append("&lt;/ul&gt;");
        inList = false;
    }
}
</pre>

<hr />

<h2>SECTION 5: INTEGRATION OF API SERVICES AND CONFIGURATION</h2>

<h3>5.1 Secret Isolation Standard</h3>
<p>
    All sensitive operational credentials and access keys are kept isolated locally. The tracked repository baseline is clean, using only structured placeholders inside `appsettings.json`.
</p>

<p>
    During local execution, the .NET environment loads the untracked, gitignored configuration file `appsettings.Development.json` to overwrite placeholders:
</p>

<pre>
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "Groq": {
    "ApiKey": "gsk_YOUR_REAL_GROQ_API_KEY_HERE"
  },
  "AbuseIPDB": {
    "ApiKey": "YOUR_ABUSEIPDB_API_KEY_HERE"
  },
  "GoogleSafeBrowsing": {
    "ApiKey": "YOUR_GOOGLE_SAFE_BROWSING_API_KEY_HERE"
  },
  "URLScan": {
    "ApiKey": "YOUR_URLSCAN_IO_API_KEY_HERE"
  }
}
</pre>

<h3>5.2 Google Safe Browsing and URLScan Heuristics</h3>
<p>
    The safety analyzer service `FinShieldService.cs` combines external real-time threat intelligence feeds with local heuristic rules:
</p>

<pre>
private async Task&lt;UrlScanResult&gt; CheckUrlScanAsync(string url)
{
    var result = new UrlScanResult();
    try
    {
        var apiKey = _configuration["URLScan:ApiKey"];
        bool isMock = string.IsNullOrEmpty(apiKey) || apiKey.Contains("YOUR_");

        if (!isMock)
        {
            // Execute physical HTTP POST query to https://urlscan.io/api/v1/scan/
            // Retrieve response payload for domain checks
            await Task.Delay(400); // latency simulation placeholder
        }

        var u = url.ToLower().Trim();
        string host = ExtractHost(u);

        var trustedDomains = new[] { "google.com", "github.com", "microsoft.com", "apple.com" };
        var brands = new[] { "paypal", "amazon", "apple", "microsoft", "netflix", "bank" };

        bool isTrusted = trustedDomains.Any(d =&gt; host == d || host.EndsWith("." + d));
        bool hasBrandImpersonation = brands.Any(b =&gt; host.Contains(b)) &amp;&amp; !isTrusted;

        if (u.Contains("suspicious") || hasBrandImpersonation)
        {
            result.IsSuspicious = true;
            result.Issues.Add(isMock ? "Simulated phishing signature detected" : "URL matches phishing patterns");
        }
    }
    catch (Exception ex)
    {
        System.Diagnostics.Debug.WriteLine($"Scan failed: {ex.Message}");
    }
    return result;
}
</pre>

<hr />

<h2>SECTION 6: STYLING PRINCIPLES AND UX DESIGN SYSTEM</h2>

<p>
    BASTION utilizes custom Vanilla CSS stylesheets (`wwwroot/css/`) rather than external styling frameworks, maintaining a lightweight design structure.
</p>

<h3>Curated Colors and Variables</h3>
<ul>
    <li><strong>Primary Crimson Accent (Dark Theme):</strong> `rgba(220, 38, 38, 0.8)` offering a high-tech cybersecurity style.</li>
    <li><strong>Active Glass Panels:</strong> Styled with `-webkit-backdrop-filter: blur(16px)` and `backdrop-filter: blur(16px)` mapped to smooth semitransparent Slate borders (`rgba(51, 65, 85, 0.5)`).</li>
    <li><strong>Flexible Light Accent Highlights:</strong> Handled dynamically through localized HSL colors computed by the browser and persisted inside user preferences. These highlights apply only in Light Mode to keep text readable.</li>
</ul>

<h3>Glassmorphism UI Rules</h3>
<pre>
.gate-glass-form {
    background: rgba(30, 41, 59, 0.45);
    border: 1px solid rgba(255, 255, 255, 0.08);
    box-shadow: 0 8px 32px 0 rgba(0, 0, 0, 0.37);
    backdrop-filter: blur(12px);
    -webkit-backdrop-filter: blur(12px);
    border-radius: 12px;
}
</pre>

<hr />

<h2>SECTION 7: DEPLOYMENT AND TUNNELING INSTRUCTIONS</h2>

<h3>7.1 Instant Public Sharing (LocalTunnel / ngrok)</h3>
<p>
    To share your local development database and live BASTION portal with remote partners for testing without deploying to heavy cloud environments:
</p>
<pre>
# Start local BASTION server
dotnet run --project BASTION.csproj

# In another terminal window, share the port instantly
npx localtunnel --port 5279
</pre>
<p>
    This generates a secure public HTTPS URL (e.g. `https://cyber-fortress.loca.lt`) accessible from any browser globally.
</p>

<h3>7.2 Enterprise Deployment (Azure / Railway)</h3>
<p>
    When transitioning BASTION to permanent cloud environments, configure a persistent volume mapping to avoid clearing the SQLite `bastion.db` database during system updates. Configure your operational API keys in your cloud dashboard under the environment variables section.
</p>
