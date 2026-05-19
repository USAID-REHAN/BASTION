using Microsoft.EntityFrameworkCore;
using BASTION.Models;

namespace BASTION.Data;

/// <summary>
/// Single SQLite DbContext with all entity sets.
/// Shared across all modules — one migration history.
/// </summary>
public class BastionDbContext : DbContext
{
    public BastionDbContext(DbContextOptions<BastionDbContext> options) : base(options) { }

    // ── LexGuard ──
    public DbSet<LexGuardAnalysis> LexGuardAnalyses => Set<LexGuardAnalysis>();

    // ── Auth ──
    public DbSet<UserAccount> Users => Set<UserAccount>();

    // ── HackerLab ──
    public DbSet<UserBadge> UserBadges => Set<UserBadge>();
    public DbSet<Lab> Labs => Set<Lab>();
    public DbSet<UserLabCompletion> UserLabCompletions => Set<UserLabCompletion>();

    // ── FinShield ──
    public DbSet<UrlScanLog> UrlScanLogs => Set<UrlScanLog>();
    public DbSet<PaymentPageAnalysis> PaymentPageAnalyses => Set<PaymentPageAnalysis>();

    // ── CyberShield ──
    public DbSet<SecurityScanLog> SecurityScanLogs => Set<SecurityScanLog>();

    // ── Infrastructure ──
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<UserPreference> UserPreferences => Set<UserPreference>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<UserSession> UserSessions => Set<UserSession>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<LexGuardAnalysis>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.DocumentName).IsRequired().HasMaxLength(256);
            entity.Property(e => e.DocumentType).IsRequired().HasMaxLength(10);
            entity.Property(e => e.RiskLevel).HasMaxLength(20);
            entity.Property(e => e.UserId).IsRequired().HasMaxLength(128);
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.AnalyzedAt);
        });

        modelBuilder.Entity<UserAccount>(entity =>
        {
            entity.HasKey(e => e.Email);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(128);
            entity.Property(e => e.FullName).IsRequired().HasMaxLength(128);
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Property(e => e.Role).IsRequired().HasMaxLength(20);
        });

        modelBuilder.Entity<UserBadge>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email);
        });

        modelBuilder.Entity<Lab>(entity =>
        {
            entity.HasKey(e => e.Id);
        });

        modelBuilder.Entity<UserLabCompletion>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email);
        });

        modelBuilder.Entity<UrlScanLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email);
        });

        modelBuilder.Entity<PaymentPageAnalysis>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email);
        });

        modelBuilder.Entity<SecurityScanLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email);
        });

        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email);
        });

        modelBuilder.Entity<UserPreference>(entity =>
        {
            entity.HasKey(e => e.Email);
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email);
        });
    }
}

