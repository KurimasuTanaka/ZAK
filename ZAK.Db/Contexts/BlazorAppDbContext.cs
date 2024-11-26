using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZAK.Db.Models;

namespace ZAK.Db;

public class BlazorAppDbContext : DbContext
{
        public virtual DbSet<ApplicationModel> applications {get; set;}
        public virtual DbSet<BrigadeModel> brigades { get; set; }
        public virtual DbSet<CoefficientModel> coefficients {get; set;}

        public virtual DbSet<AddressModel> addresses {get; set;}

        public virtual DbSet<DistrictModel> districts {get; set;}
        public virtual DbSet<AddressAliasModel> addressAliases {get; set;}
        public virtual DbSet<AddressPriorityModel> addressPriorities {get; set;}

        public virtual DbSet<AddressCoordinatesModel> coordinates {get; set;}

        public virtual DbSet<ShutdownModel> shutdowns {get; set;}

        public BlazorAppDbContext() {}
        public BlazorAppDbContext(DbContextOptions optionsBuilder) : base(optionsBuilder) {}

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging();

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        { /*
            modelBuilder.Entity<ApplicationModel>().
                HasOne(a => a.address).WithMany(a => a.applications).HasForeignKey(a => a.address).
                OnDelete(DeleteBehavior.NoAction);
                
            modelBuilder.Entity<AddressModel>().
                HasOne(a => a.district).WithMany(d => d.addresses).HasForeignKey(a => a.districtName).
                OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<AddressModel>().
                HasOne(a => a.addressAlias).WithOne().HasForeignKey<AddressAliasModel>(a => a.address).
                OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<AddressModel>().
                HasOne(a => a.addressPriority).WithOne().HasForeignKey<AddressPriorityModel>(a => a.address).
                OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AddressModel>().HasOne(a => a.coordinates).WithOne().HasForeignKey<AddressesCoordinates>(a => a.address).
                OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        */
        } 
}
