using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Shops.Data;
using Shops.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shops.Utility.Data
{
    public class DataInitial
    {
        public static void Seed(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();
                context.Database.EnsureCreated();

                //Markas
                if (!context.Markas.Any())
                {
                    context.Markas.AddRange(new List<Marka>()
                    {
                        new Marka {Name ="Nike" },
                        new Marka {Name ="Adidas" },
                    });

                }
                context.SaveChanges();

                //HumanClass
                if (!context.HumanClass.Any())
                {
                    context.HumanClass.AddRange(new List<HumanClass>()
                    {
                        new HumanClass() {Name ="Man"  },
                        new HumanClass() {Name ="Woman"  },
                    });

                }
                context.SaveChanges();

                //AgeStages
                if (!context.AgeStages.Any())
                {
                    context.AgeStages.AddRange(new List<AgeStage>()
                    {
                        new AgeStage() {Name ="Children" },
                        new AgeStage() {Name ="Young"  },
                        new AgeStage() {Name ="Adult"  },
                    });

                }
                context.SaveChanges();

                //Roles
                if (!context.Roles.Any())
                {
                    context.Roles.AddRange(new List<IdentityRole>()
                    {
                        new IdentityRole { Name="Admin" , NormalizedName ="ADMIN"},
                        new IdentityRole { Name="Casheer" , NormalizedName ="CASHEER" },
                        new IdentityRole { Name="Customer" , NormalizedName ="CUSTOMER"},
                    });

                }
                context.SaveChanges();

                //Catogery
                if (!context.ClothesClassifications.Any())
                {
                    context.ClothesClassifications.AddRange(new List<ClothesClassification>()
                    {
                        new ClothesClassification { Name="Pantalon" },
                        new ClothesClassification { Name="Te_Shirt" },
                        new ClothesClassification { Name="Short" },
                        new ClothesClassification { Name="Shirt" },
                        new ClothesClassification { Name="Shoes" },
                        new ClothesClassification { Name="Cap" },
                        new ClothesClassification { Name="Dress" },
                       
                    });

                }
                context.SaveChanges();

            }
        }
    }
}
