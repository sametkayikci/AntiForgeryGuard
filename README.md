
# AntiForgeryGuard

Automate anti-forgery protection in your ASP.NET Core MVC applications by scanning controllers and views to ensure all HTTP POST methods and forms are properly secured against Cross-Site Request Forgery (CSRF) attacks.

## Table of Contents

- [Introduction](#introduction)
- [Features](#features)
- [Prerequisites](#prerequisites)
- [Installation](#installation)
- [Usage](#usage)
    - [Processing Controllers](#processing-controllers)
    - [Processing Views](#processing-views)
- [Code Overview](#code-overview)
    - [AntiForgeryGuardService](#antiforgeryguardservice)
    - [HttpPostAnalyzer](#httppostanalyzer)
    - [AntiForgeryTokenInjector](#antiforgerytokeninjector)
- [Unit Testing](#unit-testing)
- [Limitations](#limitations)
- [Contributing](#contributing)
- [License](#license)

## Introduction

**AntiForgeryGuard** is a tool designed to enhance the security of ASP.NET Core MVC applications by automating the addition of anti-forgery protections. It scans your controller files to add `[ValidateAntiForgeryToken]` attributes to HTTP POST methods that lack them and updates your Razor views to include `asp-antiforgery="true"` in all `<form>` tags with `method="post"`.

## Features

- **Automatic Controller Scanning**: Parses controller files to identify HTTP POST methods and injects `[ValidateAntiForgeryToken]` where necessary.
- **Razor View Processing**: Scans `.cshtml` files to find `<form>` elements with `method="post"` and adds `asp-antiforgery="true"` if missing.
- **Unit Tested**: Includes unit tests using xUnit to ensure reliability.
- **Easy Integration**: Simple to set up and run in your existing projects.

## Prerequisites

- **.NET 7 or later**: The project utilizes features like `[GeneratedRegex]` which are available in .NET 7 and above.
- **ASP.NET Core MVC Project**: Designed to work with ASP.NET Core MVC applications.

## Installation

1. **Clone the Repository**:

   ```bash
   git clone https://github.com/sametkayikci/AntiForgeryGuard.git
   ```

2. **Navigate to the Project Directory**:

   ```bash
   cd AntiForgeryGuard
   ```

3. **Restore Dependencies**:

   ```bash
   dotnet restore
   ```

4. **Build the Project**:

   ```bash
   dotnet build
   ```

## Usage

You can run the `AntiForgeryGuardService` to process your controllers and views.

### Processing Controllers

Modify the `Main` method in the `Program` class to include the paths to your controller directories:

```csharp
const string controllersPath = @"C:\Projects\YourApp\Controllers";
const string areasControllersPath = @"C:\Projects\YourApp\Areas\Admin\Controllers";

await service.ProcessControllersAsync(controllersPath, areasControllersPath);
```

### Processing Views

Similarly, include the paths to your view directories:

```csharp
const string viewsPath = @"C:\Projects\YourApp\Views";
const string areasViewsPath = @"C:\Projects\YourApp\Areas\Admin\Views";

await service.ProcessViewsAsync(viewsPath, areasViewsPath);
```

### Running the Tool

Run the application:

```bash
dotnet run --project AntiForgeryGuard.csproj
```

The tool will process the specified directories, updating files as needed.

## Code Overview

### AntiForgeryGuardService

The core service that orchestrates the processing of controllers and views.

- **ProcessControllersAsync**: Scans controller files and adds `[ValidateAntiForgeryToken]` to HTTP POST methods lacking the attribute.
- **ProcessViewsAsync**: Scans Razor view files and adds `asp-antiforgery="true"` to `<form>` tags with `method="post"`.

### HttpPostAnalyzer

Analyzes C# syntax trees to identify HTTP POST methods.

```csharp
public sealed class HttpPostAnalyzer : IHttpPostAnalyzer
{
    public IEnumerable<MethodDeclarationSyntax> GetHttpPostMethods(SyntaxNode root)
    {
        // Implementation
    }
}
```

### AntiForgeryTokenInjector

Injects the `[ValidateAntiForgeryToken]` attribute into methods.

```csharp
public sealed class AntiForgeryTokenInjector : IAntiForgeryTokenInjector
{
    public MethodDeclarationSyntax AddAntiForgeryTokenAttribute(MethodDeclarationSyntax method)
    {
        // Implementation
    }
}
```

## Unit Testing

Unit tests are provided using xUnit to ensure the reliability of the tool.

### Running Tests

Navigate to the test project directory and run:

```bash
dotnet test
```

### Test Coverage

- **Form Tag Processing**: Tests the addition of `asp-antiforgery="true"` to form tags.
- **Razor Content Processing**: Tests processing of entire Razor view content.

## Limitations

- **Dynamic Forms**: The tool may not catch forms generated dynamically in code (e.g., `@using (Html.BeginForm())`).
- **Custom Attributes**: If your project uses custom method attributes instead of `[HttpPost]`, you may need to extend the analyzer.

## Contributing

Contributions are welcome! Please open issues for any bugs or feature requests, and feel free to submit pull requests.

## License

This project is licensed under the MIT License.

---

**Note**: Always back up your project before running automated tools that modify your codebase.
