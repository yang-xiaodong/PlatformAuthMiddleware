using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Authentication;

namespace WebApp.Middleware.Platform
{
    public class PlatformValidatePrincipalContext : BasePlatformContext
    {
        public PlatformValidatePrincipalContext(HttpContext context, AuthenticationTicket ticket, PlatformAuthenticationOptions options)
            : base(context, options)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (ticket == null)
            {
                throw new ArgumentNullException(nameof(ticket));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            Principal = ticket.Principal;
            Properties = ticket.Properties;
        }

        public ClaimsPrincipal Principal { get; private set; }

        public AuthenticationProperties Properties { get; private set; }

        public bool ShouldRenew { get; set; }
    }
}
