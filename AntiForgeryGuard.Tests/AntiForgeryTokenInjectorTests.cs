using System;
using System.Linq;
using Xunit;

namespace AntiForgeryGuard.Tests;

public class AntiForgeryTokenInjectorTests
{
    private readonly IAntiForgeryTokenInjector _injector = new AntiForgeryTokenInjector();

    [Fact]
    public void AddAntiForgeryTokenAttribute_Should_Add_Attribute_When_Not_Present()
    {
        // Arrange
        const string methodCode = @"
            public class TestClass
            {
                [HttpPost]
                public IActionResult SubmitData()
                {
                    return View();
                }
            }";

        var method = GetMethodDeclaration(methodCode);

        // Act
        var updatedMethod = _injector.AddAntiForgeryTokenAttribute(method);

        // Assert
        var hasAttribute = updatedMethod.AttributeLists
            .SelectMany(attrs => attrs.Attributes)
            .Any(attr => attr.Name.ToString()
                .Contains("ValidateAntiForgeryToken"));

        Assert.True(hasAttribute);
    }

    [Fact]
    public void AddAntiForgeryTokenAttribute_Should_Not_Add_Attribute_When_Already_Present()
    {
        // Arrange
        const string methodCode = @"
            public class TestClass
            {
                [HttpPost]
                [ValidateAntiForgeryToken]
                public IActionResult SubmitData()
                {
                    return View();
                }
            }";

        var method = GetMethodDeclaration(methodCode);

        // Act
        var updatedMethod = _injector.AddAntiForgeryTokenAttribute(method);

        // Assert
        var attributeCount = updatedMethod.AttributeLists
            .SelectMany(attrs => attrs.Attributes)
            .Count(attr => attr.Name.ToString()
                .Contains("ValidateAntiForgeryToken"));

        Assert.Equal(1, attributeCount);
    }

    private MethodDeclarationSyntax GetMethodDeclaration(string code)
    {
        var tree = CSharpSyntaxTree.ParseText(code);
        var root = tree.GetRoot();
      
        var method = root.DescendantNodes()
            .OfType<MethodDeclarationSyntax>()
            .FirstOrDefault();

        if (method is null)
            throw new InvalidOperationException("MethodDeclarationSyntax not found in provided code.");

        return method;
    }
}
