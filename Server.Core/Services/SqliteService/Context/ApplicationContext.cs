using Microsoft.EntityFrameworkCore;

namespace Server.Core.Services
{
    public class ApplicationContext : DbContext
    {
        public DbSet<RequestСoefficientMinMax> Сoefficients { get; set; } = null!;
        public DbSet<PredictionRow> PredictionRows { get; set; } = null!;
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=coefficients.db");
        }
    }
}
