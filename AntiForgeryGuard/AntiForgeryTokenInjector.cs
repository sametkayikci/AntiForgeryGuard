using System;

namespace AntiForgeryGuard;

public sealed class AntiForgeryTokenInjector : IAntiForgeryTokenInjector
{
    public MethodDeclarationSyntax AddAntiForgeryTokenAttribute(MethodDeclarationSyntax method)
    {
        if (method.Modifiers.Any(SyntaxKind.AsyncKeyword) && method.ReturnType.ToString().Contains("Task"))
        {
            Console.WriteLine($"Async method found: {method.Identifier.Text}");
        }

        var hasValidateAntiForgeryToken = method.AttributeLists
            .Any(list => list.Attributes
                .Any(attr => attr.Name.ToString()
                    .Contains("ValidateAntiForgeryToken")));

        if (!hasValidateAntiForgeryToken)
        {
            var validateAntiForgeryTokenAttribute = SyntaxFactory.Attribute(SyntaxFactory.ParseName(" ValidateAntiForgeryToken"));

            var attributeListWithHttpPost = method.AttributeLists
                .FirstOrDefault(list => list.Attributes
                    .Any(attr => attr.Name.ToString()
                        .Equals("HttpPost", StringComparison.OrdinalIgnoreCase)));

            if (attributeListWithHttpPost is not null)
            {
                var updatedAttributeList = attributeListWithHttpPost.AddAttributes(validateAntiForgeryTokenAttribute);
                var newAttributeLists = method.AttributeLists.Replace(attributeListWithHttpPost, updatedAttributeList);
                return method.WithAttributeLists(newAttributeLists);
            }

            var newAttributeList = SyntaxFactory.AttributeList(SyntaxFactory.SeparatedList(new[]
            {
                SyntaxFactory.Attribute(SyntaxFactory.ParseName("HttpPost")),
                validateAntiForgeryTokenAttribute
            }));

            return method.WithAttributeLists(method.AttributeLists.Add(newAttributeList));
        }

        return method;
    }
}