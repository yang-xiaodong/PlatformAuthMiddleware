using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.Middleware.Platform
{
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.DataProtection;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Options;

    public class PlatformAuthenticationOptions : AuthenticationOptions, IOptions<PlatformAuthenticationOptions>
    {
        private string _PlatformName;

        public PlatformAuthenticationOptions() {
            AuthenticationScheme = PlatformAuthenticationDefaults.AuthenticationScheme;
            AutomaticAuthenticate = true;
            ReturnUrlParameter = PlatformAuthenticationDefaults.ReturnUrlParameter;
            ExpireTimeSpan = TimeSpan.FromDays(14);
            SlidingExpiration = true;
            PlatformHttpOnly = true;
            Events = new PlatformAuthenticationEvents();
        }

        public string PlatformName {
            get { return _PlatformName; }
            set {
                if (value == null) {
                    throw new ArgumentNullException(nameof(value));
                }

                _PlatformName = value;
            }
        }
        public string PlatformDomain { get; set; }
        public string PlatformPath { get; set; }
        public bool PlatformHttpOnly { get; set; }
        public IDataProtectionProvider DataProtectionProvider { get; set; }
        public TimeSpan ExpireTimeSpan { get; set; }
        public bool SlidingExpiration { get; set; }
        public PathString LoginPath { get; set; }
        public PathString LogoutPath { get; set; }
        public PathString AccessDeniedPath { get; set; }
        public string ReturnUrlParameter { get; set; }
        public IPlatformAuthenticationEvents Events { get; set; }
        public ISecureDataFormat<AuthenticationTicket> TicketDataFormat { get; set; }
        public Microsoft.AspNetCore.Authentication.Cookies.ICookieManager CookieManager { get; set; }
        public IUserSessionStore UserSessionStore { get; set; }
        PlatformAuthenticationOptions IOptions<PlatformAuthenticationOptions>.Value {
            get {
                return this;
            }
        }
    }
}
