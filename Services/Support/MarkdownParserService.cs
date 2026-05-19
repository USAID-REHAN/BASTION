using Microsoft.AspNetCore.Components;
using System.Text;

namespace BASTION.Services.Support;

/// <summary>
/// Service responsible for parsing raw Markdown text into safe, styled HTML blocks.
/// Satisfies the Single Responsibility Principle (SRP) by decoupling text parsing from UI views.
/// </summary>
public class MarkdownParserService
{
    /// <summary>
    /// Parsers raw markdown text and returns a Microsoft.AspNetCore.Components.MarkupString.
    /// Supports stripped custom HTML headers, clean paragraph conversions, and structured lists.
    /// </summary>
    public MarkupString Parse(string rawMarkdown)
    {
        if (string.IsNullOrWhiteSpace(rawMarkdown))
            return new MarkupString(string.Empty);

        var lines = rawMarkdown.Split('\n');
        var htmlBuilder = new StringBuilder();
        bool inList = false;

        foreach (var rawLine in lines)
        {
            var line = rawLine.Trim();
            if (string.IsNullOrEmpty(line))
                continue;

            // Header level mappings
            if (line.StartsWith("####"))
            {
                CloseList(htmlBuilder, ref inList);
                htmlBuilder.Append($"<h6 class='chat-h6'>{line.TrimStart('#').Trim()}</h6>");
            }
            else if (line.StartsWith("###"))
            {
                CloseList(htmlBuilder, ref inList);
                htmlBuilder.Append($"<h5 class='chat-h5'>{line.TrimStart('#').Trim()}</h5>");
            }
            else if (line.StartsWith("##"))
            {
                CloseList(htmlBuilder, ref inList);
                htmlBuilder.Append($"<h4 class='chat-h4'>{line.TrimStart('#').Trim()}</h4>");
            }
            else if (line.StartsWith("#"))
            {
                CloseList(htmlBuilder, ref inList);
                htmlBuilder.Append($"<h3 class='chat-h3'>{line.TrimStart('#').Trim()}</h3>");
            }
            // List item mappings
            else if (line.StartsWith("-") || line.StartsWith("*"))
            {
                if (!inList)
                {
                    htmlBuilder.Append("<ul class='chat-ul'>");
                    inList = true;
                }
                htmlBuilder.Append($"<li class='chat-li'>{line.Substring(1).Trim()}</li>");
            }
            // Paragraph mappings
            else
            {
                CloseList(htmlBuilder, ref inList);
                htmlBuilder.Append($"<p class='chat-p'>{line}</p>");
            }
        }

        CloseList(htmlBuilder, ref inList);
        return new MarkupString(htmlBuilder.ToString());
    }

    private void CloseList(StringBuilder sb, ref bool inList)
    {
        if (inList)
        {
            sb.Append("</ul>");
            inList = false;
        }
    }
}
