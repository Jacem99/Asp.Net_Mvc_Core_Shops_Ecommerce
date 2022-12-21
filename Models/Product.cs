using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Shops.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(150)]
        public string Name { get; set; }

        [MaxLength(300),Required]
        public string Title { get; set; }

        [Required]
        public int Price { get; set; }

        [Required,MaxLength(6)]
        public string Size { get; set; }
        public int Rate { get; set; }

        public int? SizeNumber { get; set; }
        
     
        public byte[] Image { get; set; }
        public int? Discount { get; set; }

        //Navigation
        public int AgeStageId { get; set; }
        [ForeignKey("AgeStageId")]
        public virtual AgeStage AgeStage { get; set; }

        public int  MarkaId { get; set; }
        [ForeignKey("MarkaId")]
        public virtual Marka Marka { get; set; }

        public int HumanClassId { get; set; }
        [ForeignKey("HumanClassId")]
        public virtual HumanClass HumanClass { get; set; }

        public int ClothesClassificationId { get; set; }
        [ForeignKey("ClothesClassificationId")]
        public virtual ClothesClassification ClothesClassification { get; set; }

        public virtual ICollection<RevesationSystem> RevesationSystems { get; set; }
        public virtual ICollection<Card> Cards { get; set; }

    }
}
