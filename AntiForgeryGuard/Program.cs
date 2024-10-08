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
            const string controllersPath = @"C:\Projects\DBYS.UI\Controllers";
            const string areasAdminControllersPath = @"C:\Projects\DBYS.UI\Areas\Admin\Controllers";
            const string viewsPath = @"C:\Projects\DBYS.UI\Views";
            const string areasAdminViewsPath = @"C:\Projects\DBYS.UI\Areas\Admin\Views";

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