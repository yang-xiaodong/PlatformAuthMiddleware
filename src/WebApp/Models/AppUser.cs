using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.Models
{
    public class AppUser
    {
        public string UserName { get; set; }

        //不要在意类型
        public string Menus { get; set; }

        //不要在意类型
        public string Roles { get; set; }
    }
}
