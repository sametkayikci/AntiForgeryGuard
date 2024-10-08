using Microsoft.CodeAnalysis;
using Xunit;

namespace AntiForgeryGuard.Tests;

public class HttpPostAnalyzerTests
{
    private readonly IHttpPostAnalyzer _analyzer = new HttpPostAnalyzer();

    [Fact]
    public void GetHttpPostMethods_Should_Return_Methods_With_HttpPost_Attribute()
    {
        // Arrange
        const string code = @"
            public class SampleController
            {
                [HttpPost]
                public IActionResult SubmitData()
                {
                    return View();
                }

                [HttpGet]
                public IActionResult GetData()
                {
                    return View();
                }
            }";

        var root = GetSyntaxRoot(code);

        // Act
        var methods = _analyzer.GetHttpPostMethods(root);

        // Assert
        var methodDeclarationSyntaxes = methods.ToList();
        Assert.Single(methodDeclarationSyntaxes);
        Assert.Equal("SubmitData", methodDeclarationSyntaxes.First().Identifier.Text);
    }

    [Fact]
    public void GetHttpPostMethods_Should_Return_Empty_When_No_HttpPost_Methods()
    {
        // Arrange
        const string code = @"
            public class SampleController
            {
                [HttpGet]
                public IActionResult GetData()
                {
                    return View();
                }
            }";

        var root = GetSyntaxRoot(code);

        // Act
        var methods = _analyzer.GetHttpPostMethods(root);

        // Assert
        Assert.Empty(methods);
    }

    private SyntaxNode GetSyntaxRoot(string code)
    {
        var tree = CSharpSyntaxTree.ParseText(code);
        return tree.GetRoot();
    }
}