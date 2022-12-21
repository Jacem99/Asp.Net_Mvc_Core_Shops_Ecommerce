using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata;
using Shops.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Shops.ViewModels
{
    public class UsersRoles
    {
        public IList<string>? identityUserRoles { get; set; }

        [Display(Name ="Roles")]
        public string RoleName { get; set; }
        [Display(Name = "Roles")]
        public IList<IdentityRole>? roles { get; set; }
        public ApplicationUser? applicationUsers { get; set; }
    }
}
