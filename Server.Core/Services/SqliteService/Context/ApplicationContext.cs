using Microsoft.EntityFrameworkCore;

namespace Server.Core.Services
{
    public class ApplicationContext : DbContext
    {
        public DbSet<RequestСoefficientMinMax> Сoefficients { get; set; } = null!;
        public DbSet<PredictedRow> PredictedRows { get; set; } = null!;
        public DbSet<ClientData> ClientDataRows { get; set; } = null!;

        public ApplicationContext()
        {
            Database.EnsureDeleted();
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PredictedRow>().HasKey(key => new { key.MeteringUnitGuid, key.DateTime });
            modelBuilder.Entity<ClientData>().HasKey(key => new {key.MeteringUnitGuid, key.DateTime});
            modelBuilder.Entity<RequestСoefficientMinMax>().HasKey(key => key.MeteringUnitGuid);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=data.db");
            optionsBuilder.EnableSensitiveDataLogging();
            optionsBuilder.EnableDetailedErrors();
        }
    }
}
