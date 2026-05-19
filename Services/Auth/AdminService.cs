using BASTION.Data;
using BASTION.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BASTION.Services.Auth;

public class AdminService
{
    private readonly BastionDbContext _dbContext;

    public AdminService(BastionDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    // ── User Management ──
    
    public List<UserAccount> GetAllUsers()
    {
        return _dbContext.Users.ToList();
    }

    public bool UpdateUserRole(string email, string role)
    {
        var user = _dbContext.Users.FirstOrDefault(u => u.Email == email);
        if (user == null) return false;

        user.Role = role;
        _dbContext.SaveChanges();
        
        LogAction(email, "Promote/Demote", $"Role updated to {role}");
        return true;
    }

    public bool ToggleUserLock(string email)
    {
        var user = _dbContext.Users.FirstOrDefault(u => u.Email == email);
        if (user == null) return false;

        user.IsLockedOut = !user.IsLockedOut;
        if (user.IsLockedOut)
        {
            user.LockoutEnd = DateTime.UtcNow.AddYears(100); // Practical ban

            // Revoke all active sessions in the database immediately
            var activeSessions = _dbContext.UserSessions
                .Where(s => s.Email == email && !s.IsRevoked)
                .ToList();
            foreach (var session in activeSessions)
            {
                session.IsRevoked = true;
                session.IsRevokedByAdmin = true;
            }
            _dbContext.SaveChanges();

            // Kick them out instantly across all browsers in real-time!
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

    // ── Audit Log ──

    public List<AuditLog> GetAuditLogs()
    {
        return _dbContext.AuditLogs.OrderByDescending(l => l.Timestamp).ToList();
    }

    public void LogAction(string? email, string action, string details)
    {
        var log = new AuditLog
        {
            Email = email,
            Action = action,
            Timestamp = DateTime.UtcNow,
            Details = details
        };
        _dbContext.AuditLogs.Add(log);
        _dbContext.SaveChanges();
    }

    // ── Analytics ──

    public Dictionary<string, int> GetPlatformAnalytics()
    {
        return new Dictionary<string, int>
        {
            { "TotalUsers", _dbContext.Users.Count() },
            { "AdminUsers", _dbContext.Users.Count(u => u.Role == "Admin") },
            { "BannedUsers", _dbContext.Users.Count(u => u.IsLockedOut) },
            { "TotalScans", _dbContext.UrlScanLogs.Count() + _dbContext.SecurityScanLogs.Count() + _dbContext.PaymentPageAnalyses.Count() }
        };
    }

    // ── System Configuration ──

    private static Dictionary<string, string> _systemConfig = new()
    {
        { "MaintenanceMode", "False" },
        { "RegistrationEnabled", "True" },
        { "DefaultUserRole", "User" }
    };

    public Dictionary<string, string> GetSystemConfig()
    {
        return _systemConfig;
    }

    public void UpdateSystemConfig(string key, string value)
    {
        if (_systemConfig.ContainsKey(key))
        {
            _systemConfig[key] = value;
            LogAction("admin@bastion.app", "ConfigChange", $"Updated {key} to {value}");
        }
    }
}
