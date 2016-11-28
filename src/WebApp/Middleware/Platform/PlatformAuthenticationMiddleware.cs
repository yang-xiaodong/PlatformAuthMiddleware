using Microsoft.AspNetCore.Authentication;
using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;


namespace WebApp.Middleware.Platform
{
    using WebApp.Services;

    public class PlatformAuthenticationMiddleware : AuthenticationMiddleware<PlatformAuthenticationOptions>
    {
        private readonly IPlatformService _platformService;

        public PlatformAuthenticationMiddleware(
            RequestDelegate next,
            IDataProtectionProvider dataProtectionProvider,
            ILoggerFactory loggerFactory,
            UrlEncoder urlEncoder,
            IOptions<PlatformAuthenticationOptions> options,
            IPlatformService platformService)
            : base(next, options, loggerFactory, urlEncoder) {

            if (platformService == null) {
                throw new ArgumentNullException(nameof(platformService));
            }
            _platformService = platformService;

            if (dataProtectionProvider == null) {
                throw new ArgumentNullException(nameof(dataProtectionProvider));
            }

            if (Options.Events == null) {
                Options.Events = new PlatformAuthenticationEvents();
            }
            if (string.IsNullOrEmpty(Options.PlatformName)) {
                Options.PlatformName = PlatformAuthenticationDefaults.PlatformPrefix + Options.AuthenticationScheme;
            }
            if (Options.TicketDataFormat == null) {

                //此处应该和平台约定一套加密解密的算法，然后实现掉IDataProtectionProvider接口
                var provider = Options.DataProtectionProvider ?? dataProtectionProvider;
                var dataProtector = provider.CreateProtector(typeof(PlatformAuthenticationMiddleware).FullName, Options.AuthenticationScheme, "v2");
                Options.TicketDataFormat = new TicketDataFormat(dataProtector);
            }
            if (Options.CookieManager == null) {
                Options.CookieManager = new ChunkingCookieManager();
            }
            if (!Options.LogoutPath.HasValue) {
                Options.LogoutPath = PlatformAuthenticationDefaults.LogoutPath;
            }
            if (!Options.AccessDeniedPath.HasValue) {
                Options.AccessDeniedPath = PlatformAuthenticationDefaults.AccessDeniedPath;
            } 
        }

        protected override AuthenticationHandler<PlatformAuthenticationOptions> CreateHandler() {
            return new PlatformAuthenticationHandler(_platformService);
        }
    }
}
