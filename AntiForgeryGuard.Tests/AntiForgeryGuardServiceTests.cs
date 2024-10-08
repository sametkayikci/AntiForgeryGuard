using System.Text.RegularExpressions;
using Xunit;

namespace AntiForgeryGuard.Tests;

public class AntiForgeryGuardServiceTests
{
    [Theory]
    [InlineData("<form method=\"post\">", "<form method=\"post\" asp-antiforgery=\"true\">")]
    [InlineData("<form method='post'>", "<form method='post' asp-antiforgery=\"true\">")]
    [InlineData("<form method=\"POST\">", "<form method=\"POST\" asp-antiforgery=\"true\">")]
    [InlineData("<form method=\"post\" action=\"/Home/Submit\">", "<form method=\"post\" action=\"/Home/Submit\" asp-antiforgery=\"true\">")]
    [InlineData("<form action=\"/Home/Submit\" method=\"post\">", "<form action=\"/Home/Submit\" method=\"post\" asp-antiforgery=\"true\">")]
    [InlineData("<form method=\"post\" asp-antiforgery=\"true\">", "<form method=\"post\" asp-antiforgery=\"true\">")] // Zaten attribute mevcut
    [InlineData("<form method=\"get\">", "<form method=\"get\">")] // Method GET ise
    [InlineData("<form>", "<form>")] // Method attribute yoksa
    public void Should_Add_Antiforgery_Attribute_To_Post_Method_Form_Tags(string inputFormTag, string expectedOutputFormTag)
    {
        // Arrange
        var match = Regex.Match(inputFormTag, "<form\\b[^>]*>", RegexOptions.IgnoreCase);

        // Act
        var outputFormTag = AntiForgeryGuardService.AddAntiForgeryAttributeToFormTag(match);

        // Assert
        Assert.Equal(expectedOutputFormTag, outputFormTag);
    }

    [Theory]
    [InlineData("<form method=\"post\"></form>", "<form method=\"post\" asp-antiforgery=\"true\"></form>")]
    [InlineData("<form method=\"post\"><input type=\"text\" /></form>", "<form method=\"post\" asp-antiforgery=\"true\"><input type=\"text\" /></form>")]
    [InlineData("<form method=\"get\"></form>", "<form method=\"get\"></form>")]
    [InlineData("<form></form>", "<form></form>")]
    [InlineData("<FORM METHOD=\"POST\"></FORM>", "<FORM METHOD=\"POST\" asp-antiforgery=\"true\"></FORM>")]
    [InlineData("<form method=\"post\" asp-antiforgery=\"true\"></form>", "<form method=\"post\" asp-antiforgery=\"true\"></form>")]
    public void Should_Process_RazorContent_And_Add_Antiforgery_Attribute_When_Necessary(string inputContent, string expectedContent)
    {
        // Act
        var outputContent = AntiForgeryGuardService.ProcessRazorContent(inputContent);

        // Assert
        Assert.Equal(expectedContent, outputContent);
    }

    [Fact]
    public void Should_Process_Multiple_Form_Tags_In_RazorContent()
    {
        // Arrange
        const string inputContent = @"
                <form method=""post""></form>
                <form method=""get""></form>
                <form method=""post"" asp-antiforgery=""true""></form>
                <form></form>";

        const string expectedContent = @"
                <form method=""post"" asp-antiforgery=""true""></form>
                <form method=""get""></form>
                <form method=""post"" asp-antiforgery=""true""></form>
                <form></form>";

        // Act
        var outputContent = AntiForgeryGuardService.ProcessRazorContent(inputContent);

        // Assert
        Assert.Equal(expectedContent, outputContent);
    }

    [Fact]
    public void Should_Not_Modify_Content_When_No_Form_Tags_Present()
    {
        // Arrange
        const string inputContent = @"<div><p>Form yok!</p></div>";

        // Act
        var outputContent = AntiForgeryGuardService.ProcessRazorContent(inputContent);

        // Assert
        Assert.Equal(inputContent, outputContent);
    }

    [Fact]
    public void Should_Handle_Forms_With_Complex_Attributes_Correctly()
    {
        // Arrange
        const string inputContent = @"<form id=""myForm"" class=""form-class"" method=""post"" data-test=""value""></form>";
        const string expectedContent = @"<form id=""myForm"" class=""form-class"" method=""post"" data-test=""value"" asp-antiforgery=""true""></form>";

        // Act
        var outputContent = AntiForgeryGuardService.ProcessRazorContent(inputContent);

        // Assert
        Assert.Equal(expectedContent, outputContent);
    }
}
