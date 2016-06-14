using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using WebApp.Services;

namespace WebApp.Middleware.Platform
{
    public class PlatformAuthenticationEvents : IPlatformAuthenticationEvents
    {

        public Func<PlatformSigningOutContext, Task> OnSigningOut { get; set; } = context => Task.FromResult(0);

        public Func<PlatformRedirectContext, Task> OnRedirectToLogin { get; set; } = context =>
        {
            if (IsAjaxRequest(context.Request))
            {
                context.Response.Headers["Location"] = context.RedirectUri;
                context.Response.StatusCode = 401;
            }
            else
            {
                context.Response.Redirect(context.RedirectUri);
            }
            return Task.FromResult(0);
        };

        public Func<PlatformRedirectContext, Task> OnRedirectToAccessDenied { get; set; } = context =>
        {
            if (IsAjaxRequest(context.Request))
            {
                context.Response.Headers["Location"] = context.RedirectUri;
                context.Response.StatusCode = 403;
            }
            else
            {
                context.Response.Redirect(context.RedirectUri);
            }
            return Task.FromResult(0);
        };

        public Func<PlatformRedirectContext, Task> OnRedirectToLogout { get; set; } = context =>
        {
            if (IsAjaxRequest(context.Request))
            {
                context.Response.Headers["Location"] = context.RedirectUri;
            }
            else
            {
                context.Response.Redirect(context.RedirectUri);
            }
            return Task.FromResult(0);
        };

        public Func<PlatformRedirectContext, Task> OnRedirectToReturnUrl { get; set; } = context =>
        {
            if (IsAjaxRequest(context.Request))
            {
                context.Response.Headers["Location"] = context.RedirectUri;
            }
            else
            {
                context.Response.Redirect(context.RedirectUri);
            }
            return Task.FromResult(0);
        };

        private static bool IsAjaxRequest(HttpRequest request)
        {
            return string.Equals(request.Query["X-Requested-With"], "XMLHttpRequest", StringComparison.Ordinal) ||
                string.Equals(request.Headers["X-Requested-With"], "XMLHttpRequest", StringComparison.Ordinal);
        }

        public virtual Task SigningOut(PlatformSigningOutContext context) => OnSigningOut(context);

        public virtual Task RedirectToLogout(PlatformRedirectContext context) => OnRedirectToLogout(context);

        public virtual Task RedirectToLogin(PlatformRedirectContext context) => OnRedirectToLogin(context);

        public virtual Task RedirectToReturnUrl(PlatformRedirectContext context) => OnRedirectToReturnUrl(context);

        public virtual Task RedirectToAccessDenied(PlatformRedirectContext context) => OnRedirectToAccessDenied(context);

        public Task ValidatePrincipal(PlatformValidatePrincipalContext context, IPlatformService platformService) {
           return Task.FromResult(platformService.CheckValid(context.Principal));
        }
    }
}