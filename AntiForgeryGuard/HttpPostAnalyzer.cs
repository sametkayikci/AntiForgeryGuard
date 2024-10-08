using System;
using System.Collections.Generic;
using System.Linq;

namespace AntiForgeryGuard;

public sealed class HttpPostAnalyzer : IHttpPostAnalyzer
{
    public IEnumerable<MethodDeclarationSyntax> GetHttpPostMethods(SyntaxNode root)
    {
        return root.DescendantNodes()
            .OfType<MethodDeclarationSyntax>()
            .Where(method => method.AttributeLists
                .SelectMany(attrs => attrs.Attributes)
                .Any(attr => attr.Name.ToString()
                    .Contains("HttpPost", StringComparison.OrdinalIgnoreCase)));
    }
}