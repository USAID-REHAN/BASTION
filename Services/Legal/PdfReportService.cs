using BASTION.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace BASTION.Services.Legal;

/// <summary>
/// QuestPDF report generation — on-demand browser download.
/// Generates branded, formatted PDF of the full analysis.
/// </summary>
public class PdfReportService
{
    /// <summary>
    /// Generate a branded PDF report from analysis data.
    /// Returns the PDF as a byte array for browser download.
    /// </summary>
    public byte[] GenerateReport(AnalysisResult analysis, string documentName, DateTime analyzedAt)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(1.5f, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(10).FontColor(Colors.Grey.Darken3));

                // ── Header ──
                page.Header().Element(c => ComposeHeader(c, documentName, analyzedAt));

                // ── Content ──
                page.Content().Element(c => ComposeContent(c, analysis));

                // ── Footer ──
                page.Footer().Element(ComposeFooter);
            });
        });

        using var stream = new MemoryStream();
        document.GeneratePdf(stream);
        return stream.ToArray();
    }

    /// <summary>
    /// Generate a PDF from a Smart Legal Draft.
    /// </summary>
    public byte[] GenerateDraftPdf(string draftContent)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(11).FontColor(Colors.Black));

                page.Header().PaddingBottom(15).Row(row =>
                {
                    row.RelativeItem().Column(c =>
                    {
                        c.Item().Text("BASTION — Smart Legal Draft").FontSize(16).Bold().FontColor(Colors.Amber.Darken2);
                        c.Item().Text("AI-Generated Document").FontSize(9).FontColor(Colors.Grey.Medium);
                    });
                    row.ConstantItem(120).AlignRight().Text($"{DateTime.UtcNow:MMM dd, yyyy}").FontSize(10).FontColor(Colors.Grey.Medium);
                });

                page.Content().Element(c => RenderMarkdownToQuestPdf(c, draftContent));

                page.Footer().Element(ComposeFooter);
            });
        });

        using var stream = new MemoryStream();
        document.GeneratePdf(stream);
        return stream.ToArray();
    }

    private void RenderMarkdownToQuestPdf(IContainer container, string markdown)
    {
        container.Column(col =>
        {
            var lines = markdown.Split('\n');
            foreach (var line in lines)
            {
                var trimmed = line.Trim();
                if (string.IsNullOrEmpty(trimmed))
                {
                    col.Item().Height(8);
                    continue;
                }

                if (trimmed.StartsWith("### "))
                {
                    col.Item().PaddingTop(8).PaddingBottom(4).Text(t => RenderInlineMarkdown(t, trimmed[4..], s => s.FontSize(13).Bold().FontColor(Colors.Amber.Darken3)));
                }
                else if (trimmed.StartsWith("## "))
                {
                    col.Item().PaddingTop(10).PaddingBottom(6).Text(t => RenderInlineMarkdown(t, trimmed[3..], s => s.FontSize(15).Bold().FontColor(Colors.Amber.Darken3)));
                }
                else if (trimmed.StartsWith("# "))
                {
                    col.Item().PaddingTop(12).PaddingBottom(8).Text(t => RenderInlineMarkdown(t, trimmed[2..], s => s.FontSize(18).Bold().FontColor(Colors.Amber.Darken3)));
                }
                else if (trimmed.StartsWith("#### "))
                {
                    col.Item().PaddingTop(6).PaddingBottom(2).Text(t => RenderInlineMarkdown(t, trimmed[5..], s => s.FontSize(11).Bold().FontColor(Colors.Grey.Darken3)));
                }
                else if (trimmed.StartsWith("- ") || trimmed.StartsWith("* "))
                {
                    col.Item().PaddingBottom(4).PaddingLeft(10).Row(row =>
                    {
                        row.ConstantItem(15).Text("•").FontSize(11).FontColor(Colors.Grey.Medium);
                        row.RelativeItem().Text(t => RenderInlineMarkdown(t, trimmed[2..], s => s.FontSize(11)));
                    });
                }
                else
                {
                    col.Item().PaddingBottom(4).Text(t => RenderInlineMarkdown(t, trimmed, s => s.FontSize(11)));
                }
            }
        });
    }

    private void RenderInlineMarkdown(TextDescriptor descriptor, string text, Action<TextSpanDescriptor> styleModifier)
    {
        // Handle inline bold (**text**)
        var parts = text.Split("**");
        for (int i = 0; i < parts.Length; i++)
        {
            TextSpanDescriptor span;
            if (i % 2 == 1 && i < parts.Length - 1)
            {
                span = descriptor.Span(parts[i]).Bold();
            }
            else if (i % 2 == 1 && i == parts.Length - 1)
            {
                span = descriptor.Span("**" + parts[i]);
            }
            else
            {
                span = descriptor.Span(parts[i]);
            }
            
            styleModifier?.Invoke(span);
        }
    }

    private void ComposeHeader(IContainer container, string documentName, DateTime analyzedAt)
    {
        container.Column(col =>
        {
            col.Item().Row(row =>
            {
                row.RelativeItem().Column(c =>
                {
                    c.Item().Text("BASTION — LexGuard Analysis Report")
                        .FontSize(18).Bold().FontColor(Colors.Amber.Darken2);
                    c.Item().Text("AI-Powered Document Protection")
                        .FontSize(9).FontColor(Colors.Grey.Medium);
                });

                row.ConstantItem(120).AlignRight().Column(c =>
                {
                    c.Item().Text($"Date: {analyzedAt:yyyy-MM-dd}").FontSize(8).FontColor(Colors.Grey.Medium);
                    c.Item().Text($"Time: {analyzedAt:HH:mm} UTC").FontSize(8).FontColor(Colors.Grey.Medium);
                });
            });

            col.Item().PaddingVertical(5).LineHorizontal(2).LineColor(Colors.Amber.Darken2);

            col.Item().PaddingTop(5).Row(row =>
            {
                row.RelativeItem().Text($"Document: {documentName}")
                    .FontSize(10).SemiBold().FontColor(Colors.Grey.Darken2);
            });
        });
    }

    private void ComposeContent(IContainer container, AnalysisResult analysis)
    {
        container.PaddingVertical(10).Column(col =>
        {
            col.Spacing(12);

            // ── Risk Score ──
            col.Item().Element(c => ComposeRiskScore(c, analysis.RiskScore, analysis.OverallRiskLevel));

            // ── Summary ──
            col.Item().Element(c => ComposeSection(c, "📋 Executive Summary", analysis.Summary));

            // ── Clause Analysis ──
            col.Item().Element(c => ComposeClauses(c, analysis.Clauses));

            // ── Obligations ──
            if (analysis.Obligations.Any())
                col.Item().Element(c => ComposeObligations(c, analysis.Obligations));

            // ── Missing Clauses ──
            if (analysis.MissingClauses.Any())
                col.Item().Element(c => ComposeMissingClauses(c, analysis.MissingClauses));
        });
    }

    private void ComposeRiskScore(IContainer container, int score, string level)
    {
        var (bgColor, textColor, label) = level.ToLower() switch
        {
            "danger" => (Colors.Red.Lighten4, Colors.Red.Darken2, "HIGH RISK"),
            "caution" => (Colors.Amber.Lighten4, Colors.Amber.Darken3, "MODERATE RISK"),
            _ => (Colors.Green.Lighten4, Colors.Green.Darken2, "LOW RISK")
        };

        container.Background(bgColor).Padding(12).Row(row =>
        {
            row.RelativeItem().Column(c =>
            {
                c.Item().Text("Overall Risk Assessment").FontSize(12).Bold().FontColor(textColor);
                c.Item().Text(label).FontSize(10).FontColor(textColor);
            });
            row.ConstantItem(80).AlignCenter().AlignMiddle()
                .Text($"{score}/100").FontSize(22).Bold().FontColor(textColor);
        });
    }

    private void ComposeSection(IContainer container, string title, string content)
    {
        container.Column(col =>
        {
            col.Item().Text(title).FontSize(13).Bold().FontColor(Colors.Grey.Darken3);
            col.Item().PaddingTop(4).Text(content).FontSize(10).LineHeight(1.5f);
        });
    }

    private void ComposeClauses(IContainer container, List<ClauseAnalysis> clauses)
    {
        container.Column(col =>
        {
            col.Item().Text("⚖️ Clause-by-Clause Analysis").FontSize(13).Bold().FontColor(Colors.Grey.Darken3);
            col.Item().PaddingTop(6);

            foreach (var clause in clauses)
            {
                var borderColor = clause.RiskLevel.ToLower() switch
                {
                    "danger" => Colors.Red.Medium,
                    "caution" => Colors.Amber.Medium,
                    _ => Colors.Green.Medium
                };

                var riskLabel = clause.RiskLevel.ToLower() switch
                {
                    "danger" => "⚠️ DANGER",
                    "caution" => "⚡ CAUTION",
                    _ => "✅ SAFE"
                };

                col.Item().PaddingBottom(6).BorderLeft(3).BorderColor(borderColor).PaddingLeft(10).Column(inner =>
                {
                    inner.Item().Row(r =>
                    {
                        r.RelativeItem().Text(clause.Title).FontSize(11).SemiBold();
                        r.ConstantItem(80).AlignRight().Text(riskLabel).FontSize(8).FontColor(borderColor);
                    });
                    if (!string.IsNullOrEmpty(clause.Content))
                        inner.Item().PaddingTop(2).Text($"\"{clause.Content}\"").FontSize(9).Italic().FontColor(Colors.Grey.Darken1);
                    inner.Item().PaddingTop(2).Text(clause.Explanation).FontSize(9).LineHeight(1.4f);
                });
            }
        });
    }

    private void ComposeObligations(IContainer container, List<ObligationInfo> obligations)
    {
        container.Column(col =>
        {
            col.Item().Text("📅 Obligations & Deadlines").FontSize(13).Bold().FontColor(Colors.Grey.Darken3);
            col.Item().PaddingTop(6).Table(table =>
            {
                table.ColumnsDefinition(cols =>
                {
                    cols.RelativeColumn(3); // Title
                    cols.RelativeColumn(2); // Date
                    cols.RelativeColumn(1.5f); // Type
                    cols.RelativeColumn(4); // Description
                });

                table.Header(header =>
                {
                    header.Cell().Background(Colors.Grey.Lighten3).Padding(4).Text("Obligation").FontSize(9).SemiBold();
                    header.Cell().Background(Colors.Grey.Lighten3).Padding(4).Text("Date/Timeframe").FontSize(9).SemiBold();
                    header.Cell().Background(Colors.Grey.Lighten3).Padding(4).Text("Type").FontSize(9).SemiBold();
                    header.Cell().Background(Colors.Grey.Lighten3).Padding(4).Text("Description").FontSize(9).SemiBold();
                });

                foreach (var ob in obligations)
                {
                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(ob.Title).FontSize(9);
                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(ob.Date).FontSize(9);
                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(ob.Type.ToUpper()).FontSize(8).FontColor(Colors.Amber.Darken2);
                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(ob.Description).FontSize(9);
                }
            });
        });
    }

    private void ComposeMissingClauses(IContainer container, List<MissingClause> missing)
    {
        container.Column(col =>
        {
            col.Item().Text("🔍 Missing Protective Clauses").FontSize(13).Bold().FontColor(Colors.Grey.Darken3);
            col.Item().PaddingTop(6);

            foreach (var m in missing)
            {
                var importanceColor = m.Importance.ToLower() switch
                {
                    "high" => Colors.Red.Medium,
                    "medium" => Colors.Amber.Medium,
                    _ => Colors.Blue.Medium
                };

                col.Item().PaddingBottom(4).Row(row =>
                {
                    row.ConstantItem(6).Height(6).Background(importanceColor);
                    row.RelativeItem().PaddingLeft(8).Column(inner =>
                    {
                        inner.Item().Text($"{m.Title} ({m.Importance.ToUpper()})").FontSize(10).SemiBold().FontColor(importanceColor);
                        inner.Item().Text(m.Explanation).FontSize(9).LineHeight(1.4f);
                    });
                });
            }
        });
    }

    private void ComposeFooter(IContainer container)
    {
        container.Column(col =>
        {
            col.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
            col.Item().PaddingTop(4).Row(row =>
            {
                row.RelativeItem().Text("Generated by BASTION LexGuard — AI-Powered Document Protection")
                    .FontSize(7).FontColor(Colors.Grey.Medium);
                row.RelativeItem().AlignRight().Text(text =>
                {
                    text.Span("Page ").FontSize(7).FontColor(Colors.Grey.Medium);
                    text.CurrentPageNumber().FontSize(7).FontColor(Colors.Grey.Medium);
                    text.Span(" / ").FontSize(7).FontColor(Colors.Grey.Medium);
                    text.TotalPages().FontSize(7).FontColor(Colors.Grey.Medium);
                });
            });
        });
    }
}
