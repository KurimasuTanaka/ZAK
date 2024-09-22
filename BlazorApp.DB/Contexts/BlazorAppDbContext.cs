using BlazorApp.DB.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlazorApp.DB;

public class BlazorAppDbContext : DbContext
{
        public virtual DbSet<ApplicationModel> applications {get; set;}
        public virtual DbSet<DistrictModel> districts { get; set; }
        public virtual DbSet<BrigadeModel> brigades { get; set; }
        public virtual DbSet<CoefficientModel> coefficients {get; set;}
        public virtual DbSet<AddressModel> addresses {get; set;}


        public BlazorAppDbContext() {}
        public BlazorAppDbContext(DbContextOptions optionsBuilder) : base(optionsBuilder) {}

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging();

            base.OnConfiguring(optionsBuilder);
        }
}
