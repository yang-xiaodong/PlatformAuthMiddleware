using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;

namespace WebApp.Middleware.Platform
{
    public class PlatformSigningOutContext : BasePlatformContext
    {
        public PlatformSigningOutContext(
            HttpContext context, 
            PlatformAuthenticationOptions options, 
            AuthenticationProperties properties,
            CookieOptions cookieOptions)
            : base(context, options)
        {
            CookieOptions = cookieOptions;
            Properties = properties;
        }

        public CookieOptions CookieOptions { get; set; }

        public AuthenticationProperties Properties { get; set; }
    }
}
