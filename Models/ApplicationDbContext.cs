using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Shops.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shops.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<ApplicationUser>()
                .Property(a => a.DisplayName)
                .HasComputedColumnSql("[FirstName]+' '+ [LastName]");

            builder.Entity<HumanClass>()
                .HasMany(a => a.ApplicationUsers)
                .WithOne(h => h.HumanClass).OnDelete(DeleteBehavior.SetNull);
        }

        public DbSet<Product> products { get; set; }
        public DbSet<Card> Cards { get; set; }
        public DbSet<AgeStage> AgeStages { get; set; }
        public DbSet<ClothesClassification> ClothesClassifications { get; set; }
        public DbSet<HumanClass> HumanClass { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Marka> Markas { get; set; } 
        public DbSet<PaymentDetail> paymentDetails { get; set; }
        public DbSet<RevesationSystem> RevesationSystems { get; set; }

    }
}
