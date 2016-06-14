using System.Threading.Tasks;
using WebApp.Services;

namespace WebApp.Middleware.Platform
{
    public interface IPlatformAuthenticationEvents
    {
        Task ValidatePrincipal(PlatformValidatePrincipalContext context, IPlatformService platformService);

        Task RedirectToLogout(PlatformRedirectContext context);

        Task RedirectToLogin(PlatformRedirectContext context);

        Task RedirectToReturnUrl(PlatformRedirectContext context);

        Task RedirectToAccessDenied(PlatformRedirectContext context);

        Task SigningOut(PlatformSigningOutContext context);
    }
}
