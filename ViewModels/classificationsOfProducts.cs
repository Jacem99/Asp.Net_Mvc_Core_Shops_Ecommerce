using Shops.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using static Shops.ViewModels.ViewProducts;

namespace Shops.ViewModels
{
    public class classificationsOfProducts
    {
        [Display(Name ="Age Stage")]
        public int AgeStagesId { get; set; }
        public  IEnumerable<AgeStage> AgeStages { get; set; }


        [Display(Name = "Human Class")]
        public int HumanClassificationsId { get; set; }
        public IEnumerable<HumanClass> HumanClassifications { get; set; }


        [Display(Name = "Clothes Class")]
        public int ClothesClassificationsId { get; set; }
        public IEnumerable<ClothesClassification> ClothesClassifications { get; set; }


        public IEnumerable<Product> Product { get; set; }
        public int cardId { get; set; }
        public IEnumerable<Card> Cards { get; set; }
    }
}
