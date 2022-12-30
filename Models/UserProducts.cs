using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Shops.Models
{
    public class UserProducts
    {
        public string IdentityUserId { get; set; }
        [ForeignKey("IdentityUserId")]
        public virtual ApplicationUser ApplicationUser { get; set; }

        public virtual Product Product { get; set; }
        [ForeignKey("ProductId")]
        public int ProductId { get; set; } 

    }
}
