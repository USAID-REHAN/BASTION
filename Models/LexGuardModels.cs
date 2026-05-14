using System.Text.Json.Serialization;

namespace BASTION.Models;

// ── EF Core Entity ──

/// <summary>
/// Persisted analysis record stored in SQLite via EF Core.
/// </summary>
public class LexGuardAnalysis
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string DocumentName { get; set; } = string.Empty;
    public string DocumentType { get; set; } = string.Empty; // "pdf" or "docx"
    public long DocumentSizeBytes { get; set; }
    public DateTime AnalyzedAt { get; set; } = DateTime.UtcNow;
    public int RiskScore { get; set; }
    public string RiskLevel { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string ClausesJson { get; set; } = "[]";
    public string ObligationsJson { get; set; } = "[]";
    public string MissingClausesJson { get; set; } = "[]";
}

// ── DTOs (deserialized from Groq JSON response) ──

public class AnalysisResult
{
    [JsonPropertyName("summary")]
    public string Summary { get; set; } = string.Empty;

    [JsonPropertyName("riskScore")]
    public int RiskScore { get; set; }

    [JsonPropertyName("overallRiskLevel")]
    public string OverallRiskLevel { get; set; } = "safe";

    [JsonPropertyName("clauses")]
    public List<ClauseAnalysis> Clauses { get; set; } = new();

    [JsonPropertyName("obligations")]
    public List<ObligationInfo> Obligations { get; set; } = new();

    [JsonPropertyName("missingClauses")]
    public List<MissingClause> MissingClauses { get; set; } = new();
}

public class ClauseAnalysis
{
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;

    [JsonPropertyName("riskLevel")]
    public string RiskLevel { get; set; } = "safe";

    [JsonPropertyName("explanation")]
    public string Explanation { get; set; } = string.Empty;
}

public class ObligationInfo
{
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("date")]
    public string Date { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;
}

public class MissingClause
{
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("importance")]
    public string Importance { get; set; } = "medium";

    [JsonPropertyName("explanation")]
    public string Explanation { get; set; } = string.Empty;
}
