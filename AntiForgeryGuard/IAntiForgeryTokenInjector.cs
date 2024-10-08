namespace AntiForgeryGuard;

public interface IAntiForgeryTokenInjector
{
    MethodDeclarationSyntax AddAntiForgeryTokenAttribute(MethodDeclarationSyntax method);
}