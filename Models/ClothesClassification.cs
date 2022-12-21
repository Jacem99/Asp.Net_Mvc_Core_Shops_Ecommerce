using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Shops.Models
{
    public class ClothesClassification
    {
        [Key]
        public int Id { get; set; }
        [Required, MaxLength(200)]
        public string Name { get; set; }

        //Navigation
        public virtual ICollection<Product> Products { get; set; }

    }
}
