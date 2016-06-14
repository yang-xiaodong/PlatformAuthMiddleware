using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebApp.Models;

namespace WebApp.Services
{
    //平台接口
    public interface IPlatformService
    {

        bool CheckValid(ClaimsPrincipal ticket);

        AppUser GetPlatformUser(string userName);

        void SignOut(string userName);
    }
}
