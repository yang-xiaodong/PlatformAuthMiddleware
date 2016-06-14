using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApp.Middleware.Platform;
using WebApp.Models;

namespace WebApp.Services
{
    public class UserSessionStore : IUserSessionStore
    {
        public Task<AppUser> GetAsync(string key) {
            return Task.FromResult(new AppUser());
        }

        public Task RemoveAsync(string key) {
            return Task.FromResult(0);
        }

        public Task RenewAsync(string key, AppUser user) {
            return Task.FromResult(0);
        }
    }
}
