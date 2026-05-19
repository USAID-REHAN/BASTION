using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BASTION.Data;
using BASTION.Models;
using BASTION.Services.AI;
using BASTION.Services.Auth;
using BASTION.Services.Theme;
using BASTION.Services.Sound;

namespace BASTION.Services.Support;

/// <summary>
/// Autonomous Agentic Support Service.
/// Implements a secure ReAct (Reason + Act) loop using Groq to autonomously execute C# tools.
/// </summary>
public class SupportAgentService
{
    private readonly BastionDbContext _dbContext;
    private readonly AuthService _authService;
    private readonly AdminService _adminService;
    private readonly GroqService _groqService;
    private readonly ThemeService _themeService;
    private readonly SoundService _soundService;

    public SupportAgentService(
        BastionDbContext dbContext,
        AuthService authService,
        AdminService adminService,
        GroqService groqService,
        ThemeService themeService,
        SoundService soundService)
    {
        _dbContext = dbContext;
        _authService = authService;
        _adminService = adminService;
        _groqService = groqService;
        _themeService = themeService;
        _soundService = soundService;
    }

    private const string SystemPrompt = @"You are BASTION-Sentinel, the autonomous AI support engineer inside the BASTION Digital Fortress.
Your objective is to help the user diagnose issues and manage active configurations by using your tools.

You have access to the following C# tools to help you investigate:
1. `GetActiveSessions` - Retrieves details of all active platform login sessions.
   Format to call: [TOOL_CALL: GetActiveSessions]

2. `GetSystemSettings` - Retrieves system hardening configurations (MaintenanceMode, RegistrationEnabled, DefaultUserRole).
   Format to call: [TOOL_CALL: GetSystemSettings]

3. `ReactivateUserSession` - Reactivates a terminated/revoked login session.
   Parameter: User email address to reactivate.
   Format to call: [TOOL_CALL: ReactivateUserSession, email@example.com]

4. `ToggleSetting` - Toggles system configuration keys (e.g. MaintenanceMode or RegistrationEnabled).
   Parameters: key, value (e.g. MaintenanceMode, True or RegistrationEnabled, False).
   Format to call: [TOOL_CALL: ToggleSetting, KeyName, Value]

5. `SetSystemTheme` - Toggles the platform theme between Light Mode and Dark Mode dynamically.
   Parameter: themeMode (either 'light' or 'dark').
   Format to call: [TOOL_CALL: SetSystemTheme, themeMode]

6. `PlaySystemSound` - Triggers a platform test sound effect (beep or chime) dynamically.
   Parameter: soundType (either 'beep' or 'chime').
   Format to call: [TOOL_CALL: PlaySystemSound, soundType]

7. `SetBackgroundMusic` - Changes the platform background music (BGM) selection or silences it.
   Parameter: trackName (either 'bgm1' for Track 1 Cyber Ambient, 'bgm2' for Track 2 Neon Synth, or 'none' for Silence).
   Format to call: [TOOL_CALL: SetBackgroundMusic, trackName]

INSTRUCTIONS ON TOOLS:
- CRITICAL: If a user describes an issue and requests a fix (e.g. 'FIX IT', 'enable registration', 'turn off maintenance mode', 'make me admin'), you MUST NOT stop at diagnosis. You MUST actively invoke the `ToggleSetting` or `ReactivateUserSession` tool to apply the correction in C# before writing your final answer.
- You can call ONLY ONE tool per response. Output the tool call block on a single line, with absolutely no other text, introduction, or explanation around it:
  [TOOL_CALL: ToolName, Param1, Param2...]
- After calling a tool, the system will execute it and provide the results in a [TOOL_RESULT] block. You can then analyze the result and decide to either call another tool or write the final answer.

FINAL ANSWER INSTRUCTIONS:
- Once you have the results and are ready to provide the final resolution to the user, write:
  [FINAL_ANSWER] followed by your helpful, friendly, and detailed response explaining what was wrong, what actions were taken, and how the user's issue has been resolved.
- Always use clear, premium, Markdown formatting in your final answer.";

    /// <summary>
    /// Processes a live support chat message or ticket autonomously through the AI Agent reasoning loop.
    /// </summary>
    public async Task<(string Response, List<string> AgentSteps)> ProcessAgenticTaskAsync(string userMessage, List<GroqChatMessage> history)
    {
        var steps = new List<string>();
        var chatMessages = new List<GroqChatMessage>
        {
            new() { Role = "system", Content = SystemPrompt }
        };

        // Append historical context if present
        foreach (var msg in history)
        {
            chatMessages.Add(msg);
        }

        // Add user message if not already there
        if (!chatMessages.Any(m => m.Content == userMessage && m.Role == "user"))
        {
            chatMessages.Add(new GroqChatMessage { Role = "user", Content = userMessage });
        }

        int loopCount = 0;
        const int maxLoops = 4; // Absolute safety cap to prevent loop overflows

        while (loopCount < maxLoops)
        {
            loopCount++;
            var aiResponse = await _groqService.ChatAsync(chatMessages, temperature: 0.1);

            // Parse response for tool calls
            if (aiResponse.Contains("[TOOL_CALL:"))
            {
                var toolCall = ExtractTag(aiResponse, "[TOOL_CALL:", "]");
                steps.Add($"Agent Decided: {toolCall}");

                var result = await ExecuteToolAsync(toolCall);
                steps.Add($"Agent Executed: {toolCall} => Result: {result}");

                // Add the AI's reasoning and the tool result to conversation history
                chatMessages.Add(new GroqChatMessage { Role = "assistant", Content = aiResponse });
                chatMessages.Add(new GroqChatMessage { Role = "user", Content = $"[TOOL_RESULT: {result}]" });
            }
            else
            {
                // No more tool calls, we have the final answer!
                string cleanResponse = aiResponse;
                if (aiResponse.Contains("[FINAL_ANSWER]"))
                {
                    cleanResponse = aiResponse.Substring(aiResponse.IndexOf("[FINAL_ANSWER]") + "[FINAL_ANSWER]".Length).Trim();
                }
                return (cleanResponse, steps);
            }
        }

        return ("I've investigated your issue through our security tools, but need administrative confirmation to complete this action.", steps);
    }

    /// <summary>
    /// Executes the specified tool in C# on the server side safely.
    /// </summary>
    private async Task<string> ExecuteToolAsync(string toolCall)
    {
        try
        {
            var parts = toolCall.Split(',', StringSplitOptions.TrimEntries);
            var toolName = parts[0];

            switch (toolName.ToLower())
            {
                case "getactivesessions":
                    var sessions = _authService.GetAllSessions();
                    var activeList = sessions.Select(s => $"{s.UserEmail} ({s.Device} - {s.Location}) [Active: {s.IsActive}, AdminRevoked: {s.IsRevokedByAdmin}]");
                    return activeList.Any() 
                        ? $"Active Sessions: {string.Join(" | ", activeList)}" 
                        : "No active user sessions found.";

                case "getsystemsettings":
                    var configs = _adminService.GetSystemConfig();
                    return $"System Configuration: {string.Join(", ", configs.Select(c => $"{c.Key}={c.Value}"))}";

                case "reactivateusersession":
                    if (parts.Length < 2) return "Error: Missing email parameter.";
                    var emailToReactivate = parts[1].ToLower();
                    
                    // Search DB session and reactivate
                    var dbSession = _dbContext.UserSessions
                        .OrderByDescending(s => s.LoginTime)
                        .FirstOrDefault(s => s.Email == emailToReactivate && s.IsRevoked);

                    if (dbSession != null)
                    {
                        dbSession.IsRevoked = false;
                        dbSession.IsRevokedByAdmin = false;
                        await _dbContext.SaveChangesAsync();
                        
                        _adminService.LogAction("BASTION-Sentinel", "AgentReactivate", $"Session reactivated for {emailToReactivate}");
                        return $"Success: Reactivated latest login session for {emailToReactivate}.";
                    }
                    return $"Notice: No revoked session found for user {emailToReactivate}.";

                case "togglesetting":
                    if (parts.Length < 3) return "Error: Missing key or value parameters.";
                    var key = parts[1];
                    var val = parts[2];

                    var allowedKeys = new[] { "MaintenanceMode", "RegistrationEnabled", "DefaultUserRole" };
                    if (!allowedKeys.Contains(key))
                    {
                        return $"Error: Config key '{key}' is unauthorized for agent modifications.";
                    }

                    _adminService.UpdateSystemConfig(key, val);
                    _adminService.LogAction("BASTION-Sentinel", "AgentConfigToggle", $"Toggled {key} to {val}");
                    return $"Success: System configuration key '{key}' has been updated to '{val}'.";

                case "setsystemtheme":
                    if (parts.Length < 2) return "Error: Missing theme parameter.";
                    var themeParam = parts[1].ToLower();
                    if (themeParam == "light")
                    {
                        await _themeService.SetDarkMode(false);
                        return "Success: BASTION theme updated to Light Mode.";
                    }
                    else if (themeParam == "dark")
                    {
                        await _themeService.SetDarkMode(true);
                        return "Success: BASTION theme updated to Dark Mode.";
                    }
                    return "Error: Theme mode must be 'light' or 'dark'.";

                case "playsystemsound":
                    if (parts.Length < 2) return "Error: Missing soundType parameter.";
                    var soundType = parts[1].ToLower();
                    if (soundType == "chime")
                    {
                        await _soundService.PlayChime();
                        return "Success: Triggered system chime sound effect.";
                    }
                    else if (soundType == "beep")
                    {
                        await _soundService.PlayBeep();
                        return "Success: Triggered system beep sound effect.";
                    }
                    return "Error: Sound type must be 'chime' or 'beep'.";

                case "setbackgroundmusic":
                    if (parts.Length < 2) return "Error: Missing track name parameter.";
                    var trackName = parts[1].ToLower();
                    var validTracks = new[] { "bgm1", "bgm2", "none" };
                    if (!validTracks.Contains(trackName))
                    {
                        return "Error: Background music track must be 'bgm1', 'bgm2', or 'none'.";
                    }
                    await _soundService.SetBgm(trackName);
                    string trackDesc = trackName == "none" ? "silenced background audio" : $"started playing BGM '{trackName}'";
                    return $"Success: Successfully {trackDesc}.";

                default:
                    return $"Error: Tool '{toolName}' is undefined.";
            }
        }
        catch (Exception ex)
        {
            return $"Error executing tool: {ex.Message}";
        }
    }

    private static string ExtractTag(string text, string startTag, string endTag)
    {
        int start = text.IndexOf(startTag);
        if (start == -1) return string.Empty;
        start += startTag.Length;
        int end = text.IndexOf(endTag, start);
        if (end == -1) return string.Empty;
        return text.Substring(start, end - start).Trim();
    }
}
