using System;
using System.Threading.Tasks;

namespace AntiForgeryGuard;

public static class Program
{
    private static async Task Main()
    {
        var analyzer = new HttpPostAnalyzer();
        var injector = new AntiForgeryTokenInjector();
        var service = new AntiForgeryGuardService(analyzer, injector);

        try
        {
            const string controllersPath = @"C:\Projects\MyApp\Controllers";
            const string areasAdminControllersPath = @"C:\Projects\MyApp\Areas\Admin\Controllers";
            const string viewsPath = @"C:\Projects\MyApp\Views";
            const string areasAdminViewsPath = @"C:\Projects\MyApp\Areas\Admin\Views";

            await service.ProcessControllersAsync(controllersPath, areasAdminControllersPath);
            await service.ProcessViewsAsync(viewsPath, areasAdminViewsPath);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
