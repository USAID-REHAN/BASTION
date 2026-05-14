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
    }
}
