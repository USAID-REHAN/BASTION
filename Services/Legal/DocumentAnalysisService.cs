using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using BASTION.Data;
using BASTION.Models;
using BASTION.Services.AI;
using DocumentFormat.OpenXml.Packaging;
using UglyToad.PdfPig;

namespace BASTION.Services.Legal;

/// <summary>
/// Groq AI call + response parser for legal document analysis.
/// Extracts text from PDF/DOCX, sends to Groq, parses structured JSON response.
/// </summary>
public class DocumentAnalysisService
{
    private readonly GroqService _groq;
    private readonly BastionDbContext _db;

    private const string SystemPrompt = @"You are LexGuard, an expert legal document analyst within the BASTION security suite. Analyze the provided document text and identify all important legal clauses, risks, obligations, and missing protections.

Return ONLY a valid JSON object (no markdown, no code blocks, no explanation outside JSON) with this EXACT structure:
{
  ""summary"": ""A clear 2-3 sentence plain-English summary of the document and its implications for the signing party."",
  ""riskScore"": <number 0-100 where 0=completely safe and 100=extremely dangerous>,
  ""overallRiskLevel"": ""<safe|caution|danger>"",
  ""clauses"": [
    {
      ""title"": ""Short descriptive title"",
      ""content"": ""Relevant excerpt or paraphrase from the document"",
      ""riskLevel"": ""<safe|caution|danger>"",
      ""explanation"": ""Why this clause matters and what risk it poses""
    }
  ],
  ""obligations"": [
    {
      ""title"": ""Obligation name"",
      ""date"": ""Specific date or timeframe like 'Within 30 days' or 'Ongoing'"",
      ""type"": ""<deadline|payment|renewal|termination|notification>"",
      ""description"": ""What you must do or be aware of""
    }
  ],
  ""missingClauses"": [
    {
      ""title"": ""Missing clause name"",
      ""importance"": ""<high|medium|low>"",
      ""explanation"": ""Why this clause should be included""
    }
  ]
}

Rules:
- Identify ALL meaningful clauses (aim for at least 5 if the document is substantial)
- Flag genuinely risky clauses as danger, potentially concerning as caution, fair/standard as safe
- Extract ALL time-bound obligations (deadlines, payments, renewals, terminations)
- List important protective clauses that are MISSING
- Explain in plain English, avoid unnecessary legal jargon
- The riskScore should reflect overall danger: 0-30=safe, 31-60=caution, 61-100=danger";

    public DocumentAnalysisService(GroqService groq, BastionDbContext db)
    {
        _groq = groq;
        _db = db;
    }

    /// <summary>
    /// Extract text from a PDF file stream.
    /// </summary>
    public string ExtractTextFromPdf(Stream stream)
    {
        var sb = new StringBuilder();
        using var document = PdfDocument.Open(stream);
        foreach (var page in document.GetPages())
        {
            sb.AppendLine(page.Text);
        }
        return sb.ToString().Trim();
    }

    /// <summary>
    /// Extract text from a DOCX file stream.
    /// </summary>
    public string ExtractTextFromDocx(Stream stream)
    {
        var sb = new StringBuilder();
        using var doc = WordprocessingDocument.Open(stream, false);
        var body = doc.MainDocumentPart?.Document?.Body;
        if (body != null)
        {
            foreach (var paragraph in body.Elements<DocumentFormat.OpenXml.Wordprocessing.Paragraph>())
            {
                sb.AppendLine(paragraph.InnerText);
            }
        }
        return sb.ToString().Trim();
    }

    /// <summary>
    /// Send extracted text to Groq for analysis and parse the structured response.
    /// </summary>
    public async Task<AnalysisResult> AnalyzeTextAsync(string documentText)
    {
        // Truncate extremely long documents (Groq has token limits)
        if (documentText.Length > 60000)
        {
            documentText = documentText[..60000] + "\n\n[Document truncated due to length — analysis covers the first ~60,000 characters]";
        }

        var rawResponse = await _groq.ChatAsync(SystemPrompt, documentText, 0.2);

        // Try to extract JSON from the response (Groq sometimes wraps in markdown)
        var jsonString = ExtractJson(rawResponse);

        try
        {
            var result = JsonSerializer.Deserialize<AnalysisResult>(jsonString);
            return result ?? CreateFallbackResult(rawResponse);
        }
        catch (JsonException)
        {
            return CreateFallbackResult(rawResponse);
        }
    }

    /// <summary>
    /// Full pipeline: extract text, analyze, and persist to database.
    /// </summary>
    public async Task<(AnalysisResult Result, LexGuardAnalysis Record, string RawText)> AnalyzeDocumentAsync(
        Stream fileStream, string fileName, string fileType, long fileSize, string userId)
    {
        // 1. Extract text
        string text = fileType.ToLower() switch
        {
            "pdf" => ExtractTextFromPdf(fileStream),
            "docx" => ExtractTextFromDocx(fileStream),
            _ => throw new NotSupportedException($"Unsupported file type: {fileType}")
        };

        if (string.IsNullOrWhiteSpace(text))
            throw new InvalidOperationException("Could not extract any text from the document. The file may be scanned/image-based or empty.");

        // 2. Analyze with Groq
        var result = await AnalyzeTextAsync(text);

        // 3. Persist to SQLite
        var record = new LexGuardAnalysis
        {
            UserId = userId,
            DocumentName = fileName,
            DocumentType = fileType,
            DocumentSizeBytes = fileSize,
            AnalyzedAt = DateTime.UtcNow,
            RiskScore = Math.Clamp(result.RiskScore, 0, 100),
            RiskLevel = result.OverallRiskLevel,
            Summary = result.Summary,
            ClausesJson = JsonSerializer.Serialize(result.Clauses),
            ObligationsJson = JsonSerializer.Serialize(result.Obligations),
            MissingClausesJson = JsonSerializer.Serialize(result.MissingClauses)
        };

        _db.LexGuardAnalyses.Add(record);
        _db.AuditLogs.Add(new AuditLog
        {
            Email = userId,
            Action = "LexGuard Document Analysis",
            Timestamp = DateTime.UtcNow,
            Details = $"Analyzed {fileName} with risk score {record.RiskScore}"
        });
        await _db.SaveChangesAsync();

        return (result, record, text);
    }

    /// <summary>
    /// Retrieve a past analysis by ID and deserialize its JSON fields.
    /// </summary>
    public AnalysisResult? DeserializeAnalysis(LexGuardAnalysis record)
    {
        try
        {
            return new AnalysisResult
            {
                Summary = record.Summary,
                RiskScore = record.RiskScore,
                OverallRiskLevel = record.RiskLevel,
                Clauses = JsonSerializer.Deserialize<List<ClauseAnalysis>>(record.ClausesJson) ?? new(),
                Obligations = JsonSerializer.Deserialize<List<ObligationInfo>>(record.ObligationsJson) ?? new(),
                MissingClauses = JsonSerializer.Deserialize<List<MissingClause>>(record.MissingClausesJson) ?? new()
            };
        }
        catch
        {
            return null;
        }
    }

    // ── FEATURE 1: Interactive Document Q&A ──
    public async Task<string> ChatWithDocumentAsync(string documentText, string userQuestion, List<(string Role, string Content)> chatHistory)
    {
        var prompt = "You are a legal AI assistant. Answer the user's question based STRICTLY on the provided document. If the answer is not in the document, say so. Keep it concise, helpful, and in plain English.\n\nDOCUMENT:\n" + documentText;
        
        // Truncate if necessary
        if (prompt.Length > 25000) prompt = prompt[..25000] + "\n[Truncated]";

        // Format conversation history for Groq
        var conversation = new StringBuilder();
        conversation.AppendLine(prompt);
        foreach (var msg in chatHistory)
        {
            conversation.AppendLine($"{msg.Role.ToUpper()}: {msg.Content}");
        }
        conversation.AppendLine($"USER: {userQuestion}\nASSISTANT:");

        return await _groq.ChatAsync("You are a helpful legal assistant.", conversation.ToString(), 0.3);
    }

    // ── FEATURE 4: Smart Legal Drafting ──
    public async Task<string> DraftContractAsync(string parameters)
    {
        var prompt = "You are LexGuard, an expert legal drafter. Draft a comprehensive, legally sound contract or document based strictly on the following requirements. Format it using Markdown with clear headers and bullet points. Include standard protective boilerplate clauses.\n\nREQUIREMENTS:\n" + parameters;
        return await _groq.ChatAsync("You are an expert lawyer.", prompt, 0.4);
    }

    // ── FEATURE 5: Document Comparison ──
    public async Task<string> CompareDocumentsAsync(string textA, string textB)
    {
        var prompt = "Compare Version A and Version B of the following document. Do not just list text differences. Explain the LEGAL IMPLICATIONS of the changes. Which party benefits from the changes? What new risks were introduced? DO NOT use any '#' characters or markdown headings in your output. Use double asterisks for bolding, bullet points, or lists for headings/emphasis.\n\nVERSION A:\n" + textA + "\n\nVERSION B:\n" + textB;
        if (prompt.Length > 60000) prompt = prompt[..60000] + "[Truncated]";
        var response = await _groq.ChatAsync("You are an expert contract reviewer.", prompt, 0.2);
        return RemoveHashCharacters(response);
    }

    // ── FEATURE 6: Compliance Auditing ──
    public async Task<string> AuditComplianceAsync(string documentText, string framework)
    {
        var prompt = $"Audit the following document for compliance against {framework} (e.g., GDPR, CCPA, HIPAA). Identify missing required clauses, non-compliant clauses, and give actionable recommendations to fix them. DO NOT use any '#' characters or markdown headings in your output. Use double asterisks for bolding, bullet points, or lists for headings/emphasis.\n\nDOCUMENT:\n" + documentText;
        if (prompt.Length > 50000) prompt = prompt[..50000] + "[Truncated]";
        var response = await _groq.ChatAsync("You are a strict compliance auditor.", prompt, 0.1);
        return RemoveHashCharacters(response);
    }

    public static string RemoveHashCharacters(string? text)
    {
        if (string.IsNullOrEmpty(text))
            return string.Empty;

        return text
            .Replace("#", string.Empty)
            .Replace("＃", string.Empty)
            .Replace("&#35;", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("&#x23;", string.Empty, StringComparison.OrdinalIgnoreCase);
    }

    private static string ExtractJson(string text)
    {
        // Try to find JSON object in the response
        text = text.Trim();

        // Remove markdown code blocks if present
        var codeBlockMatch = Regex.Match(text, @"```(?:json)?\s*([\s\S]*?)```", RegexOptions.Singleline);
        if (codeBlockMatch.Success)
            text = codeBlockMatch.Groups[1].Value.Trim();

        // Find first { and last }
        var start = text.IndexOf('{');
        var end = text.LastIndexOf('}');
        if (start >= 0 && end > start)
            return text[start..(end + 1)];

        return text;
    }

    private static AnalysisResult CreateFallbackResult(string rawText)
    {
        return new AnalysisResult
        {
            Summary = "The AI analysis could not be fully parsed. Raw response: " + rawText[..Math.Min(500, rawText.Length)],
            RiskScore = 50,
            OverallRiskLevel = "caution",
            Clauses = new List<ClauseAnalysis>
            {
                new() { Title = "Analysis Note", Content = "The AI response was not in the expected format.", RiskLevel = "caution", Explanation = "Please try re-analyzing the document." }
            }
        };
    }
}
