using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Shops.Models
{
    public class Card
    {
        public int Id { get; set; }
        public Nullable<bool> Buyed { get; set; }
        public Nullable<bool> Favourite { get; set; }
        public int mount { get; set; }

        //Navigate
        public String IdentityUserId { get; set; }
        [ForeignKey("IdentityUserId")]
        public virtual ApplicationUser ApplicationUsers { get; set; }

        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public virtual Product Products { get; set; }

    }
}
