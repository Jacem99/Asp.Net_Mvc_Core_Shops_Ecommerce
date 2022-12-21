using Shops.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Shops.ViewModels
{
    public class ViewProducts
    {
      public Product product { get; set; }

        //Navigation
        public int AgeStageId { get; set; }
        public IEnumerable<AgeStage> AgeStage { get; set; }
        public int MarkaId { get; set; }
        public IEnumerable<Marka> Markas { get; set; }
        public int ClothesClassificationId { get; set; }
        public IEnumerable<ClothesClassification> ClothesClassifications { get; set; }
       }
}
