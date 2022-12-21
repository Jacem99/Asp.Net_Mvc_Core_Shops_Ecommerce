using Microsoft.AspNetCore.Identity;
using Shops.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shops.ViewModels
{
    public class CutomerRoles
    {
        public string Roles { get; set; }
        public IList<IdentityRole> identityRoles { get; set; }
        public IList<ApplicationUser> applicationUsers { get; set; }
    }
}
