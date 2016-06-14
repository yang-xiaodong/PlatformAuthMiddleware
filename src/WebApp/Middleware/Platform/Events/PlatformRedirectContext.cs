using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;

namespace WebApp.Middleware.Platform
{
    public class PlatformRedirectContext : BasePlatformContext
    {
        public PlatformRedirectContext(HttpContext context, PlatformAuthenticationOptions options, string redirectUri, AuthenticationProperties properties)
            : base(context, options)
        {
            RedirectUri = redirectUri;
            Properties = properties;
        }

        public string RedirectUri { get; set; }

        public AuthenticationProperties Properties { get; }
    }
}
