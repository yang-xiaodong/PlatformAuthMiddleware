using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebApp.Models;

namespace WebApp.Services
{
    public class PlatformService : IPlatformService
    {
        public bool CheckValid(ClaimsPrincipal ticket) {
            return true;
        }

        public AppUser GetPlatformUser(string userName) {
            return new AppUser();
        }

        public void SignOut(string userName) {
            
        }
    }
}
