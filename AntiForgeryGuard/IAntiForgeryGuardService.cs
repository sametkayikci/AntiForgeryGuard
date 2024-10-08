using System.Threading.Tasks;

namespace AntiForgeryGuard;

public interface IAntiForgeryGuardService
{
    Task ProcessControllersAsync(params string[] controllerPaths);
    Task ProcessViewsAsync(params string[] viewPaths);
}