using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZAK.Db.Models;

namespace ZAK.Db;

public class ZakDbContext : DbContext
{
    public virtual DbSet<ApplicationModel> applications { get; set; }
    public virtual DbSet<BrigadeModel> brigades { get; set; }
    public virtual DbSet<CoefficientModel> coefficients { get; set; }

    public virtual DbSet<AddressModel> addresses { get; set; }

    public virtual DbSet<DistrictModel> districts { get; set; }
    public virtual DbSet<AddressAliasModel> addressAliases { get; set; }
    public virtual DbSet<AddressPriorityModel> addressPriorities { get; set; }

    public virtual DbSet<AddressCoordinatesModel> coordinates { get; set; }

    public virtual DbSet<BlackoutModel> shutdowns { get; set; }

    public ZakDbContext() { }
    public ZakDbContext(DbContextOptions optionsBuilder) : base(optionsBuilder) { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.EnableSensitiveDataLogging();

        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AddressModel>().
            HasIndex(ad => new { ad.building, ad.streetName }).IsUnique();

        modelBuilder.Entity<BrigadeModel>().Property(b => b.id).ValueGeneratedOnAdd();
        modelBuilder.Entity<AddressModel>().Property(a => a.Id).ValueGeneratedOnAdd();

        modelBuilder.Entity<ScheduledApplicationModel>().
            HasOne(sa => sa.brigade).
            WithMany(b => b.scheduledApplications).
            HasForeignKey(sa => sa.brigadeId).
            OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<CoefficientModel>().HasData([
            new CoefficientModel() { id = 1, parameter = "housePriority", parameterAlias="Пріорітетність адреси", coefficient = 1},
                new CoefficientModel() { id = 2, parameter = "distance", parameterAlias="Відстань між адресами",  coefficient = 1},
                new CoefficientModel() { id = 3, parameter = "urgency", parameterAlias="Терміновість", coefficient = 1},
                new CoefficientModel() { id = 4, parameter = "statusCheck", parameterAlias="Цікавилися статусом заявки", coefficient = 1},
                new CoefficientModel() { id = 5, parameter = "freeCable", parameterAlias="Є наявний кабель", coefficient = 1},
                new CoefficientModel() { id = 6, parameter = "tarrifeChangeApplication", parameterAlias="Заявка на зміну тарифу", coefficient = 1},
                new CoefficientModel() { id = 7, parameter = "deadline", parameterAlias="Час до дедлайну", coefficient = 1}
        ]);
    }

}
