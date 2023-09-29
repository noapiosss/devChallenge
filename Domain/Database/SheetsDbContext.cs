using Contracts.Database;
using Microsoft.EntityFrameworkCore;

namespace Domain.Database
{
    public class SheetsDbContext : DbContext
    {
        public DbSet<Cell> Cells { get; init; }

        public SheetsDbContext() : base()
        {

        }

        public SheetsDbContext(DbContextOptions<SheetsDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            _ = modelBuilder.Entity<Cell>().HasKey(nameof(Cell.SheetId), nameof(Cell.CellId));
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
           _ = optionsBuilder.UseNpgsql("Uid=postgres;Pwd=fyfnjksq123;Host=localhost:5432;Database=sheets;");
        }
    }
}