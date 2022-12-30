using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Shops.Models
{
    public class ApplicationUser :IdentityUser
    {

        public byte[]? Image{ get; set; }

        //navigation 
    
        [Display(Name = "First Name")]
        [MaxLength(60)]
        public string FirstName { get; set; } 

        [MaxLength(60)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Name")]
        [MaxLength(150)]
        public string DisplayName { get; set; }
        public virtual IEnumerable<Card> Cards { get; set; }
        public Nullable<int> HumanClassId { get; set; }
        [ForeignKey("HumanClassId")]
        public virtual HumanClass HumanClass { get; set; } 
        
        public virtual ICollection<RevesationSystem> RevesationSystems { get; set; }
        public virtual ICollection<UserProducts> UserProducts { get; set; }

    }
}
