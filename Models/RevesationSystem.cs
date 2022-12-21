using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Shops.Models
{
    public class RevesationSystem
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public DateTime Data { get; set; }

        //Navigation
        public string IdentityUserId { get; set; }
        [ForeignKey("IdentityUserId")]
        public virtual ApplicationUser ApplicationUser { get; set; }
       
        public virtual ICollection<Product> Products { get; set; }
    }
}
