using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.Middleware.Platform
{
    public static class PlatformAuthenticationDefaults
    {
        public const string AuthenticationScheme = "WLSubSystem.Application";

        public static readonly string PlatformPrefix = ".Platform.";

        public static readonly PathString LogoutPath = new PathString("/Account/Logout");

        public static readonly PathString AccessDeniedPath = new PathString("/Account/AccessDenied");

        public static readonly string ReturnUrlParameter = "ReturnUrl";
    }
}
