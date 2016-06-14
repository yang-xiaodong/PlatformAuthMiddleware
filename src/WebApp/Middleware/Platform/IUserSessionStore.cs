using Microsoft.AspNetCore.Authentication;
using System.Threading.Tasks;
using WebApp.Models;

namespace WebApp.Middleware.Platform
{
    public interface IUserSessionStore
    {
        Task RenewAsync(string key, AppUser user);

        Task<AppUser> GetAsync(string key);

        Task RemoveAsync(string key);
    }
}
