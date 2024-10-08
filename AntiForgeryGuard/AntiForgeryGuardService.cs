using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("AntiForgeryGuard.Tests")]

namespace AntiForgeryGuard;

public sealed partial class AntiForgeryGuardService(
    IHttpPostAnalyzer httpPostAnalyzer,
    IAntiForgeryTokenInjector antiForgeryTokenInjector) : IAntiForgeryGuardService
{
    public async Task ProcessControllersAsync(params string[] controllerPaths)
    {
        var controllerFiles = controllerPaths.SelectMany(path => Directory.EnumerateFiles(path, "*.cs", SearchOption.AllDirectories));

        foreach (var filePath in controllerFiles)
        {
            if (!await TryAcquireFileLockAsync(filePath))
            {
                Console.WriteLine($"File {filePath} already processed.");
                continue;
            }

            try
            {
                await using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
                fileStream.Seek(0, SeekOrigin.Begin);

                using var reader = new StreamReader(fileStream, leaveOpen: true);
                var syntaxTree = CSharpSyntaxTree.ParseText(await reader.ReadToEndAsync());
                var root = await syntaxTree.GetRootAsync();

                var httpPostMethods = httpPostAnalyzer.GetHttpPostMethods(root);
                var newRoot = root;

                foreach (var method in httpPostMethods)
                {
                    var updatedMethod = antiForgeryTokenInjector.AddAntiForgeryTokenAttribute(method);
                    newRoot = newRoot.ReplaceNode(method, updatedMethod);
                }

                if (!newRoot.IsEquivalentTo(root))
                {
                    fileStream.Seek(0, SeekOrigin.Begin);
                    fileStream.SetLength(0);

                    await using var writer = new StreamWriter(fileStream, leaveOpen: true);
                    await writer.WriteAsync(newRoot.ToFullString());
                    await writer.FlushAsync();
                    await fileStream.FlushAsync();

                    Console.WriteLine($"Updated file: {filePath}");
                }
            }
            finally
            {
                ReleaseFileLock(filePath);
            }
        }

        Console.WriteLine("Controller processing done.");
    }

    public async Task ProcessViewsAsync(params string[] viewPaths)
    {
        var viewFiles = viewPaths.SelectMany(path => Directory.EnumerateFiles(path, "*.cshtml", SearchOption.AllDirectories));

        foreach (var filePath in viewFiles)
        {
            if (!await TryAcquireFileLockAsync(filePath))
            {
                Console.WriteLine($"File {filePath} already processed.");
                continue;
            }

            try
            {
                var content = await File.ReadAllTextAsync(filePath);
                var modifiedContent = ProcessRazorContent(content);

                if (modifiedContent != content)
                {
                    await File.WriteAllTextAsync(filePath, modifiedContent);
                    Console.WriteLine($"Updated file: {filePath}");
                }
            }
            finally
            {
                ReleaseFileLock(filePath);
            }
        }

        Console.WriteLine("View processing done.");
    }

    internal static string ProcessRazorContent(string content)
    {
        var modifiedContent = FormTagRegex().Replace(content, AddAntiForgeryAttributeToFormTag);
        return modifiedContent;
    }

    internal static string AddAntiForgeryAttributeToFormTag(Match match)
    {
        var formTag = match.Value;
        if (!MethodPostRegex().IsMatch(formTag))
            return formTag;

        if (AntiForgeryRegex().IsMatch(formTag))
            return formTag;

        var updatedFormTag = formTag.TrimEnd('>') + " asp-antiforgery=\"true\">";
        return updatedFormTag;
    }

    private static async Task<bool> TryAcquireFileLockAsync(string filePath)
    {
        var lockFilePath = filePath + ".lock";
        try
        {
            await using var lockFileStream = new FileStream(lockFilePath, FileMode.CreateNew, FileAccess.Write, FileShare.None);
            return true;
        }
        catch (IOException)
        {
            return false;
        }
    }

    private static void ReleaseFileLock(string filePath)
    {
        var lockFilePath = filePath + ".lock";
        if (File.Exists(lockFilePath))
        {
            File.Delete(lockFilePath);
        }
    }

    [GeneratedRegex("\\bmethod\\s*=\\s*['\"]post['\"]", RegexOptions.IgnoreCase)]
    private static partial Regex MethodPostRegex();

    [GeneratedRegex(@"<form\b[^>]*>", RegexOptions.IgnoreCase)]
    private static partial Regex FormTagRegex();

    [GeneratedRegex(@"\basp-antiforgery\b", RegexOptions.IgnoreCase)]
    private static partial Regex AntiForgeryRegex();
}