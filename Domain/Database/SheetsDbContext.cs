using System.Linq;
using System.Runtime.InteropServices;
using Contracts.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Domain.Database
{
    public class SheetsDbContext : DbContext
    {
        public DbSet<Cell> Cells { get; init; }
        public DbSet<CellDependency> CellDependencies { get; init; }

        public SheetsDbContext() : base()
        {

        }

        public SheetsDbContext(DbContextOptions<SheetsDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            _ = modelBuilder.Entity<Cell>().HasKey(nameof(Cell.SheetId), nameof(Cell.CellId));
            _ = modelBuilder.Entity<CellDependency>()
                .HasKey(nameof(CellDependency.DependedSheetId),
                        nameof(CellDependency.DependedCellId),
                        nameof(CellDependency.DependedBySheetId),
                        nameof(CellDependency.DependedByCellId));

            _ = modelBuilder.Entity<CellDependency>()
                .HasOne(cd => cd.DependedCell)
                .WithMany(c => c.DependByCells)
                .HasForeignKey(cd => new {cd.DependedSheetId, cd.DependedCellId});

            _ = modelBuilder.Entity<CellDependency>()
                .HasOne(cd => cd.DependedByCell)
                .WithMany(c => c.DependedCells)
                .HasForeignKey(cd => new {cd.DependedBySheetId, cd.DependedByCellId});
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
           _ = optionsBuilder.UseNpgsql("Uid=postgres;Pwd=fyfnjksq123;Host=localhost:5432;Database=sheets;");
        }

        // public IQueryable<Cell> GetDependedCells(string sheedId, string dependedByCellId)  
        // {  
        //     return from cellDependency in this.CellDependencies  
        //            join dependedCell in this.Cells on cellDependency.DependedByCellId equals dependedCell.CellId
        //            where cellDependency.DependedByCellId == dependedByCellId && cellDependency.DependedBySheetId == sheedId
        //            select dependedCell;  
        // }  
  
        // public IQueryable<Cell> GetDependnies(string sheedId, string dependedCellId)  
        // {  
        //     return from dependedCells in this.CellDependencies  
        //            join dependedByCells in this.Cells on dependedCells.DependedByCellId equals dependedByCells.CellId  
        //            where dependedByCells.DependedCellId == dependedCellId && cellDependency.DependedSheetId == sheedId
        //            select dependedByCells;  
        // }  
    }
}