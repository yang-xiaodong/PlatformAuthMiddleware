using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;

namespace WebApp.Middleware.Platform
{
    public class BasePlatformContext : BaseContext
    {
        public BasePlatformContext(
            HttpContext context,
            PlatformAuthenticationOptions options)
            : base(context)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            Options = options;
        }

        public PlatformAuthenticationOptions Options { get; }
    }
}
