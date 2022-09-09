using System.Diagnostics.CodeAnalysis;
using MeterReadingsUploader.Database.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;

namespace MeterReadingsUploader.Database.EntityFramework
{
    public class DatabaseContext : DbContext
    {
        private readonly IConfiguration _configuration;

        public DatabaseContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public DbSet<MeterReadingEntity> MeterReadings { get; set; } = null!;
        public DbSet<AccountEntity> Accounts { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var databaseConnectionString = _configuration.GetConnectionString("SQLiteDatabase");
            optionsBuilder.UseSqlite($"Data Source={databaseConnectionString};");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MeterReadingEntity>().ToTable("MeterReadings");

            modelBuilder.Entity<AccountEntity>().ToTable("Accounts");
        }
    }
}
