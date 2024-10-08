using System.Collections.Generic;

namespace AntiForgeryGuard;
public interface IHttpPostAnalyzer
{
    IEnumerable<MethodDeclarationSyntax> GetHttpPostMethods(SyntaxNode root);
}
