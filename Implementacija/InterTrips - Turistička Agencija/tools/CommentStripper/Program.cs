using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

var rootDir = GetArg("--root") ?? Directory.GetCurrentDirectory();
var mode = (GetArg("--mode") ?? "all").Trim().ToLowerInvariant();

var excludeSegments = new List<string>
{
    $"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}",
    $"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}",
    $"{Path.DirectorySeparatorChar}.git{Path.DirectorySeparatorChar}",
    $"{Path.DirectorySeparatorChar}.vs{Path.DirectorySeparatorChar}",
    $"{Path.DirectorySeparatorChar}Data{Path.DirectorySeparatorChar}Migrations{Path.DirectorySeparatorChar}",
};

var excludeExt = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
{
    ".dll", ".exe", ".pdb", ".png", ".jpg", ".jpeg", ".gif", ".ico", ".svg", ".pdf", ".zip", ".7z", ".rar",
    ".woff", ".woff2", ".ttf", ".eot", ".mp4", ".mp3", ".wav", ".db", ".bak", ".br", ".gz",
};

var includeCs = mode is "all" or "cs";
var includeRazor = mode is "all" or "razor";
var includeOther = mode is "all" or "other";

var allFiles = Directory.EnumerateFiles(rootDir, "*", SearchOption.AllDirectories)
    .Where(p => !excludeSegments.Any(seg => p.Contains(seg, StringComparison.OrdinalIgnoreCase)))
    .Where(p => !excludeExt.Contains(Path.GetExtension(p)))
    .ToList();

var changed = 0;
var processed = 0;

foreach (var path in allFiles)
{
    var ext = Path.GetExtension(path);

    if (ext.Equals(".cs", StringComparison.OrdinalIgnoreCase))
    {
        if (!includeCs) continue;
    }
    else if (ext.Equals(".cshtml", StringComparison.OrdinalIgnoreCase))
    {
        if (!includeRazor) continue;
    }
    else
    {
        if (!includeOther) continue;
    }

    if (!TryReadAllText(path, out var original, out var encoding))
        continue;

    processed++;

    var updated = ext.ToLowerInvariant() switch
    {
        ".cs" => StripCSharpComments(original),
        ".cshtml" => StripRazorComments(original),
        ".js" or ".ts" or ".css" or ".scss" => StripCStyleCommentsPreservingStrings(original),
        ".html" or ".htm" or ".xml" or ".config" or ".csproj" or ".props" or ".targets" or ".md" => StripHtmlComments(original),
        ".yml" or ".yaml" or ".editorconfig" => StripHashLineComments(original),
        ".ps1" => StripPowerShellComments(original),
        _ => StripCStyleCommentsPreservingStrings(original),
    };

    if (!string.Equals(original, updated, StringComparison.Ordinal))
    {
        File.WriteAllText(path, updated, encoding);
        changed++;
    }
}

Console.WriteLine($"Processed: {processed}");
Console.WriteLine($"Changed: {changed}");

string? GetArg(string name)
{
    for (var i = 0; i < args.Length; i++)
    {
        if (!string.Equals(args[i], name, StringComparison.OrdinalIgnoreCase))
            continue;

        if (i + 1 >= args.Length)
            return "";

        return args[i + 1];
    }

    return null;
}

static bool TryReadAllText(string path, out string text, out Encoding encoding)
{
    try
    {
        using var stream = File.OpenRead(path);
        if (stream.Length == 0)
        {
            text = string.Empty;
            encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
            return true;
        }

        Span<byte> probe = stackalloc byte[1024];
        var read = stream.Read(probe);
        if (probe[..read].IndexOf((byte)0) >= 0)
        {
            text = string.Empty;
            encoding = Encoding.UTF8;
            return false;
        }

        stream.Position = 0;
        using var reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true);
        text = reader.ReadToEnd();
        encoding = reader.CurrentEncoding;
        return true;
    }
    catch
    {
        text = string.Empty;
        encoding = Encoding.UTF8;
        return false;
    }
}

static string StripCSharpComments(string text)
{
    var parseOptions = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Latest);
    var tree = CSharpSyntaxTree.ParseText(text, parseOptions);
    var root = tree.GetRoot();

    var trivias = root.DescendantTrivia(descendIntoTrivia: true)
        .Where(t => t.IsKind(SyntaxKind.SingleLineCommentTrivia)
                 || t.IsKind(SyntaxKind.MultiLineCommentTrivia)
                 || t.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia)
                 || t.IsKind(SyntaxKind.MultiLineDocumentationCommentTrivia)
                 || t.IsKind(SyntaxKind.DocumentationCommentExteriorTrivia))
        .ToArray();

    if (trivias.Length == 0)
        return text;

    var newRoot = root.ReplaceTrivia(trivias, (t, _) => SyntaxFactory.Whitespace(" "));
    return newRoot.ToFullString();
}

static string StripRazorComments(string text)
{
    var withoutRazor = ReplaceAllBalanced(text, "@*", "*@", " ");
    return StripHtmlComments(withoutRazor);
}

static string StripHtmlComments(string text)
{
    return ReplaceAllBalanced(text, "<!--", "-->", " ");
}

static string StripHashLineComments(string text)
{
    var sb = new StringBuilder(text.Length);
    using var sr = new StringReader(text);
    string? line;
    while ((line = sr.ReadLine()) is not null)
    {
        var trimmed = line.TrimStart();
        if (trimmed.StartsWith('#') || trimmed.StartsWith(';'))
        {
            sb.AppendLine();
            continue;
        }

        sb.AppendLine(line);
    }

    return sb.ToString();
}

static string StripPowerShellComments(string text)
{
    var noBlock = ReplaceAllBalanced(text, "<#", "#>", " ");
    var sb = new StringBuilder(noBlock.Length);
    using var sr = new StringReader(noBlock);
    string? line;
    while ((line = sr.ReadLine()) is not null)
    {
        var idx = line.IndexOf('#');
        if (idx >= 0)
            sb.AppendLine(line[..idx]);
        else
            sb.AppendLine(line);
    }
    return sb.ToString();
}

static string StripCStyleCommentsPreservingStrings(string text)
{
    var sb = new StringBuilder(text.Length);
    var i = 0;
    var state = 0;

    while (i < text.Length)
    {
        var c = text[i];
        var n = i + 1 < text.Length ? text[i + 1] : '\0';

        if (state == 0)
        {
            if (c == '\'' ) { state = 1; sb.Append(c); i++; continue; }
            if (c == '"' ) { state = 2; sb.Append(c); i++; continue; }
            if (c == '`' ) { state = 3; sb.Append(c); i++; continue; }

            if (c == '/' && n == '/')
            {
                i += 2;
                while (i < text.Length && text[i] != '\n') i++;
                continue;
            }

            if (c == '/' && n == '*')
            {
                i += 2;
                while (i + 1 < text.Length && !(text[i] == '*' && text[i + 1] == '/')) i++;
                if (i + 1 < text.Length) i += 2;
                sb.Append(' ');
                continue;
            }

            sb.Append(c);
            i++;
            continue;
        }

        if (state is 1 or 2)
        {
            sb.Append(c);
            if (c == '\\' && n != '\0')
            {
                sb.Append(n);
                i += 2;
                continue;
            }

            if (state == 1 && c == '\'') { state = 0; }
            else if (state == 2 && c == '"') { state = 0; }

            i++;
            continue;
        }

        if (state == 3)
        {
            sb.Append(c);
            if (c == '\\' && n != '\0')
            {
                sb.Append(n);
                i += 2;
                continue;
            }

            if (c == '`') state = 0;
            i++;
            continue;
        }
    }

    return sb.ToString();
}

static string ReplaceAllBalanced(string text, string start, string end, string replacement)
{
    var sb = new StringBuilder(text.Length);
    var idx = 0;
    while (idx < text.Length)
    {
        var startIdx = text.IndexOf(start, idx, StringComparison.Ordinal);
        if (startIdx < 0)
        {
            sb.Append(text, idx, text.Length - idx);
            break;
        }

        sb.Append(text, idx, startIdx - idx);
        var endIdx = text.IndexOf(end, startIdx + start.Length, StringComparison.Ordinal);
        if (endIdx < 0)
        {
            sb.Append(text, startIdx, text.Length - startIdx);
            break;
        }

        sb.Append(replacement);
        idx = endIdx + end.Length;
    }

    return sb.ToString();
}
