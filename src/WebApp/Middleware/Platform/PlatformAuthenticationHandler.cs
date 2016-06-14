using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Http.Features.Authentication;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebApp.Services;

namespace WebApp.Middleware.Platform
{
    public class PlatformAuthenticationHandler : AuthenticationHandler<PlatformAuthenticationOptions>
    {
        private const string HeaderValueNoCache = "no-cache";
        private const string HeaderValueMinusOne = "-1";
        private const string SessionIdClaim = "CMS.Platforms-SessionId";

        private string _sessionKey;
        private Task<AuthenticateResult> _readPlatformTask;
        private readonly IPlatformService _IPlatformService;
        public PlatformAuthenticationHandler(IPlatformService platformService) {
            _IPlatformService = platformService;
        }

        private Task<AuthenticateResult> EnsurePlatformTicket() {
            if (_readPlatformTask == null) {
                _readPlatformTask = ReadPlatformTicket();
            }
            return _readPlatformTask;
        }
        private Task<AuthenticateResult> ReadPlatformTicket() {

            //读取平台Cookie
            var platformCookie = Options.CookieManager.GetRequestCookie(Context, Options.PlatformName);
            if (string.IsNullOrEmpty(platformCookie)) {
                return Task.FromResult(AuthenticateResult.Skip());
            }

            //==============================模拟操作,=================================
            //此处应该和平台约定一套加密解密的算法，然后实现掉IDataProtectionProvider接口
            var principal = new ClaimsPrincipal();
            var authProp = new AuthenticationProperties();

            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Name, "zhangsan"));
            claims.Add(new Claim(ClaimTypes.Email, "zhangsan@qq.com"));
            var identity = new ClaimsIdentity(claims, Options.PlatformName);

            principal.AddIdentity(identity);

            principal.Claims.Append(new Claim(SessionIdClaim, "SessionId"));

            var myTicket = new AuthenticationTicket(principal, authProp, Options.AuthenticationScheme);

            var myPlatformCookie = Options.TicketDataFormat.Protect(myTicket);
            //==============================END 模拟操作=================================


            //解析Cookie中加密的票据
            var ticket = Options.TicketDataFormat.Unprotect(myPlatformCookie);
            if (ticket == null) {
                return Task.FromResult(AuthenticateResult.Fail("解密平台票据失败"));
            }

            //验证票据是否过期或有效
            var issuedUtc = ticket.Properties.IssuedUtc;
            var expiresUtc = ticket.Properties.ExpiresUtc;
            if (issuedUtc != null && expiresUtc != null && expiresUtc.Value < DateTime.UtcNow) {
                return Task.FromResult((AuthenticateResult.Fail("票据过期")));
            }

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync() {

            //获取 AuthenticateResult
            var result = await EnsurePlatformTicket();
            if (!result.Succeeded) {
                return result;
            }

            //Ticket包含很多信息。调用平台接口 Check(Ticket)验证票据是否合法,  及各种跳转逻辑
            var context = new PlatformValidatePrincipalContext(Context, result.Ticket, Options);
            if (context.Principal == null) {
                return AuthenticateResult.Fail("No principal.");
            }
            await Options.Events.ValidatePrincipal(context, _IPlatformService);

            _sessionKey = result.Ticket.Principal.Identity.Name;
            //保存User信息
            if (Options.UserSessionStore != null && _sessionKey != null) {

                //调用接口获取User信息
                var appUser = await Options.UserSessionStore.GetAsync(_sessionKey);

                if (appUser == null) {
                    appUser = _IPlatformService.GetPlatformUser(_sessionKey);
                    await Options.UserSessionStore.RenewAsync(_sessionKey, appUser);
                }
            }
            return AuthenticateResult.Success(new AuthenticationTicket(context.Principal, context.Properties, Options.AuthenticationScheme));
        }

        private CookieOptions BuildCookieOptions() {
            var cookieOptions = new CookieOptions {
                Domain = Options.PlatformDomain,
                HttpOnly = Options.PlatformHttpOnly,
                Path = Options.PlatformPath ?? (OriginalPathBase.HasValue ? OriginalPathBase.ToString() : "/"),
            };
            return cookieOptions;
        }

        protected override Task FinishResponseAsync() {
            if (SignInAccepted || SignOutAccepted) {
                return Task.FromResult(0);
            }
            return Task.FromResult(0);
        }

        protected override async Task HandleSignOutAsync(SignOutContext signOutContext) {

            var ticket = await EnsurePlatformTicket();
            var PlatformOptions = BuildCookieOptions();
            if (Options.UserSessionStore != null && _sessionKey != null) {
                await Options.UserSessionStore.RemoveAsync(_sessionKey);
            }
            _IPlatformService.SignOut(_sessionKey);

            var context = new PlatformSigningOutContext(
                Context,
                Options,
                new AuthenticationProperties(signOutContext.Properties),
                PlatformOptions);

            await Options.Events.SigningOut(context);

            Options.CookieManager.DeleteCookie(
                Context,
                Options.PlatformName,
                context.CookieOptions);
            
            // Only redirect on the logout path
            var shouldRedirect = Options.LogoutPath.HasValue && OriginalPath == Options.LogoutPath;
            await ApplyHeaders(shouldRedirect, context.Properties);
        }

        private async Task ApplyHeaders(bool shouldRedirectToReturnUrl, AuthenticationProperties properties) {
            Response.Headers[HeaderNames.CacheControl] = HeaderValueNoCache;
            Response.Headers[HeaderNames.Pragma] = HeaderValueNoCache;
            Response.Headers[HeaderNames.Expires] = HeaderValueMinusOne;
            if (shouldRedirectToReturnUrl && Response.StatusCode == 200) {
                var query = Request.Query;
                var redirectUri = query[Options.ReturnUrlParameter];
                if (!StringValues.IsNullOrEmpty(redirectUri)
                    && IsHostRelative(redirectUri)) {
                    var redirectContext = new PlatformRedirectContext(Context, Options, redirectUri, properties);
                    await Options.Events.RedirectToReturnUrl(redirectContext);
                }
            }

        }

        private static bool IsHostRelative(string path) {
            if (string.IsNullOrEmpty(path)) {
                return false;
            }
            if (path.Length == 1) {
                return path[0] == '/';
            }
            return path[0] == '/' && path[1] != '/' && path[1] != '\\';
        }

        protected override async Task<bool> HandleForbiddenAsync(ChallengeContext context) {
            var properties = new AuthenticationProperties(context.Properties);
            var returnUrl = properties.RedirectUri;
            if (string.IsNullOrEmpty(returnUrl)) {
                returnUrl = OriginalPathBase + Request.Path + Request.QueryString;
            }
            var accessDeniedUri = Options.AccessDeniedPath + QueryString.Create(Options.ReturnUrlParameter, returnUrl);
            var redirectContext = new PlatformRedirectContext(Context, Options, BuildRedirectUri(accessDeniedUri), properties);
            await Options.Events.RedirectToAccessDenied(redirectContext);
            return true;
        }

        protected override async Task<bool> HandleUnauthorizedAsync(ChallengeContext context) {
            if (context == null) {
                throw new ArgumentNullException(nameof(context));
            }

            var properties = new AuthenticationProperties(context.Properties);
            var redirectUri = properties.RedirectUri;
            if (string.IsNullOrEmpty(redirectUri)) {
                redirectUri = OriginalPathBase + Request.Path + Request.QueryString;
            }

            var loginUri = Options.LoginPath + QueryString.Create(Options.ReturnUrlParameter, redirectUri);
            var redirectContext = new PlatformRedirectContext(Context, Options, BuildRedirectUri(loginUri), properties);
            await Options.Events.RedirectToLogin(redirectContext);
            return true;

        }
    }
}
