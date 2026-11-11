using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NeuroInscryption.Communication;

/// <summary>
/// Renders game state as Markdown for Neuro.
/// </summary>
/// <remarks>
/// Objects go through Json.NET first, so <see cref="JsonPropertyAttribute"/>, <see cref="JsonIgnoreAttribute"/>
/// and <see cref="JsonExtensionDataAttribute"/> still decide which fields appear, what they're called and their order.
/// Values are written verbatim (no Markdown escaping) so identifiers match the action schemas exactly.
/// </remarks>
public static class MarkdownSerializer
{
    private const string NoneText = "None";
    private const string NullText = "Empty";

    private static readonly JsonSerializer _serializer = JsonSerializer.Create(new JsonSerializerSettings
    {
        NullValueHandling = NullValueHandling.Ignore,
    });

    private static readonly Regex _identifier = new("^[A-Za-z][A-Za-z0-9]*$");
    private static readonly Regex _wordBoundary = new("(?<=[a-z0-9])(?=[A-Z])|(?<=[A-Z])(?=[A-Z][a-z])");
    private static readonly Regex _lineBreak = new(@"\s*\r?\n\s*");

    public static string Serialize(object? value)
    {
        if (value == null) return "";

        JToken token = value as JToken ?? JToken.FromObject(value, _serializer);
        StringBuilder sb = new();

        if (token is JObject root)
        {
            // Plain values go first so they can't be read as part of the section above them
            foreach (JProperty prop in root.Properties().Where(p => p.Value is JValue))
                WriteEntry(sb, prop.Name, prop.Value, 0);

            foreach (JProperty prop in root.Properties().Where(p => p.Value is not JValue))
            {
                sb.AppendLine().Append("## ").AppendLine(Humanize(prop.Name));
                WriteBlock(sb, prop.Value);
            }
        }
        else
        {
            WriteBlock(sb, token);
        }

        return sb.ToString().Trim();
    }

    /// <summary>Writes a value as a standalone block (the body of a section).</summary>
    private static void WriteBlock(StringBuilder sb, JToken token)
    {
        switch (token)
        {
            case JObject { Count: 0 }:
            case JArray { Count: 0 }:
                sb.AppendLine(NoneText);
                break;
            case JObject obj:
                foreach (JProperty prop in obj.Properties())
                    WriteEntry(sb, prop.Name, prop.Value, 0);
                break;
            case JArray arr:
                foreach (JToken item in arr)
                    WriteEntry(sb, null, item, 0);
                break;
            default:
                sb.AppendLine(FormatScalar(token));
                break;
        }
    }

    /// <summary>
    /// Writes one list item. Plain fields of an object share its line, nested objects and lists become sub-items.
    /// </summary>
    private static void WriteEntry(StringBuilder sb, string? label, JToken value, int depth)
    {
        StringBuilder line = new();
        if (label != null) line.Append("**").Append(Humanize(label)).Append(":**");

        string inline = value switch
        {
            JObject { Count: 0 } => NoneText,
            JObject obj => string.Join("; ", obj.Properties()
                .Where(p => IsInline(p.Value))
                .Select(p => $"{Humanize(p.Name)}: {FormatInline(p.Value)}")),
            _ when IsInline(value) => FormatInline(value),
            _ => "",
        };
        if (inline.Length > 0) line.Append(line.Length > 0 ? " " : "").Append(inline);

        sb.Append(' ', depth * 2).Append('-');
        if (line.Length > 0) sb.Append(' ').Append(line);
        sb.AppendLine();

        switch (value)
        {
            case JObject obj:
                foreach (JProperty prop in obj.Properties().Where(p => !IsInline(p.Value)))
                    WriteEntry(sb, prop.Name, prop.Value, depth + 1);
                break;
            case JArray arr when !IsInline(arr):
                foreach (JToken item in arr)
                    WriteEntry(sb, null, item, depth + 1);
                break;
        }
    }

    private static bool IsInline(JToken token) => token is JValue || (token is JArray arr && arr.All(t => t is JValue));

    private static string FormatInline(JToken token) => token switch
    {
        JArray { Count: 0 } => NoneText,
        JArray arr => string.Join(", ", arr.Select(FormatScalar)),
        _ => FormatScalar(token),
    };

    private static string FormatScalar(JToken token)
    {
        if (token is not JValue value) throw new ArgumentException($"Expected a scalar but got {token.Type}", nameof(token));

        return value.Type switch
        {
            JTokenType.Null or JTokenType.Undefined => NullText,
            JTokenType.Boolean => (bool) value ? "yes" : "no",
            JTokenType.String => _lineBreak.Replace((string) value!, " "),
            _ => Convert.ToString(value.Value, CultureInfo.InvariantCulture) ?? "",
        };
    }

    /// <summary>Turns property names like <c>PlayerLanes</c> into <c>Player Lanes</c>. Anything else (card names, lane numbers) is left alone.</summary>
    private static string Humanize(string key) => _identifier.IsMatch(key) ? _wordBoundary.Replace(key, " ") : key;
}
