using BASTION.Data;
using BASTION.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BASTION.Services.Legal;

/// <summary>
/// IModule implementation for LexGuard — document protection module.
/// </summary>
public class LexGuardModule : IModule
{
    private readonly BastionDbContext _db;

    public LexGuardModule(BastionDbContext db) => _db = db;

    public string Name => "LexGuard";
    public string Icon => "📜";
    public string RouteUrl => "/lexguard";
    public string Description => "Document protection — AI contract analysis, clause highlights, PDF export";
    public string PrimaryHex => "#D97706";
    public string CssClass => "module-lex";
    public int Order => 4;

    public async Task<int> GetScoreAsync(string userId)
    {
        // Score based on recent analyses — more analyses = better protection awareness
        var analyses = await _db.LexGuardAnalyses
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.AnalyzedAt)
            .Take(5)
            .ToListAsync();

        if (!analyses.Any()) return 0;

        // Base score: 40 for having any analyses
        // +12 per analysis (up to 5 * 12 = 60)
        // Penalize high-risk documents that haven't been reviewed recently
        var baseScore = 40;
        var analysisBonus = Math.Min(analyses.Count * 12, 60);
        return Math.Min(baseScore + analysisBonus, 100);
    }
}
