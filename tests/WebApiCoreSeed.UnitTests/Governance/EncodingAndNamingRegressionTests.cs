using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace WebApiCoreSeed.UnitTests.Governance;

public sealed class EncodingAndNamingRegressionTests
{
    [Fact(DisplayName = "Arquivos ativos nao possuem mojibake conhecido")]
    public void ActiveFilesQuandoAvaliadosNaoDevemConterMojibakeConhecido()
    {
        var markers = new[] { "\u00c3", "\u00c2", "\ufffd" };
        var violations = ReadActiveTextFiles()
            .Select(file => new
            {
                File = file,
                Markers = markers.Where(marker => File.ReadAllText(file).Contains(marker, StringComparison.Ordinal)).ToArray()
            })
            .Where(result => result.Markers.Length > 0)
            .Select(result => $"{RelativePath(result.File)}: {string.Join(", ", result.Markers.Select(ToCodePoint))}")
            .ToArray();

        Assert.Empty(violations);
    }

    [Fact(DisplayName = "Arquivos ativos nao reintroduzem nomes corrigidos")]
    public void ActiveFilesQuandoAvaliadosNaoDevemConterNomesCorrigidos()
    {
        var forbiddenTerms = new[]
        {
            Join("Inte", "faces"),
            Join("Cl", "ains"),
            Join("Fluent", "Validator"),
            Join("Log", "ginEntity"),
            Join("Log", "ginService"),
            Join("ILog", "ginService"),
            Join("Log", "ginRepository"),
            Join("ILog", "ginRepository"),
            Join("Log", "ginValidation"),
            Join("Log", "ginMapping"),
            Join("Log", "gins")
        };

        var violations = ReadActiveTextFiles()
            .Select(file => new
            {
                File = file,
                Terms = forbiddenTerms.Where(term => File.ReadAllText(file).Contains(term, StringComparison.Ordinal)).ToArray()
            })
            .Where(result => result.Terms.Length > 0)
            .Select(result => $"{RelativePath(result.File)}: {string.Join(", ", result.Terms)}")
            .ToArray();

        Assert.Empty(violations);
    }

    private static IEnumerable<string> ReadActiveTextFiles()
    {
        var root = FindRepositoryRoot();
        var roots = new[]
        {
            Path.Combine(root, "README.md"),
            Path.Combine(root, "src"),
            Path.Combine(root, "tests"),
            Path.Combine(root, "docs", "openapi", "openapi-v1.json"),
            Path.Combine(root, "docs", "openapi", "openapi-v2.json")
        };

        foreach (var path in roots)
        {
            if (File.Exists(path))
            {
                yield return path;
                continue;
            }

            foreach (var file in Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories)
                         .Where(IsActiveTextFile))
            {
                yield return file;
            }
        }
    }

    private static bool IsActiveTextFile(string file)
    {
        var normalized = file.Replace(Path.DirectorySeparatorChar, '/');
        if (normalized.Contains("/bin/", StringComparison.Ordinal)
            || normalized.Contains("/obj/", StringComparison.Ordinal)
            || normalized.Contains("/Migrations/", StringComparison.Ordinal)
            || normalized.EndsWith("packages.lock.json", StringComparison.Ordinal))
        {
            return false;
        }

        return file.EndsWith(".cs", StringComparison.Ordinal)
            || file.EndsWith(".md", StringComparison.Ordinal)
            || file.EndsWith(".json", StringComparison.Ordinal);
    }

    private static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "WebApiCoreSeed.slnx")))
        {
            directory = directory.Parent;
        }

        return directory?.FullName
            ?? throw new InvalidOperationException("Repository root not found.");
    }

    private static string RelativePath(string file)
    {
        return Path.GetRelativePath(FindRepositoryRoot(), file);
    }

    private static string Join(string left, string right) => left + right;

    private static string ToCodePoint(string marker) => $"U+{char.ConvertToUtf32(marker, 0):X4}";
}
