using System.Collections.Generic;
using System.Linq;
using Contracts.Database;
using Microsoft.EntityFrameworkCore;

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
                .HasKey(nameof(CellDependency.SheetId),
                        nameof(CellDependency.DependedCellId),
                        nameof(CellDependency.DependedByCellId));

            _ = modelBuilder.Entity<CellDependency>()
                .HasOne(cd => cd.DependedCell)
                .WithMany(c => c.DependByCells)
                .HasForeignKey(cd => new {cd.SheetId, cd.DependedCellId});

            _ = modelBuilder.Entity<CellDependency>()
                .HasOne(cd => cd.DependedByCell)
                .WithMany(c => c.DependedCells)
                .HasForeignKey(cd => new {cd.SheetId, cd.DependedByCellId});
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //_ = optionsBuilder.UseNpgsql("Uid=postgres;Pwd=123456;Server=postgres;Port=5432;Database=sheets;");
            // _ = optionsBuilder.UseNpgsql("Uid=postgres;Pwd=fyfnjksq123;Host=localhost:5432;Database=sheets;");
        }

        public IQueryable<Cell> GetAllDependencies(string sheetId, ICollection<string> dependedCellIds)  
        {
            List<string> filters = new(dependedCellIds.Count);
            foreach(string cellId in dependedCellIds)
            {
                filters.Add($"depended_cell_id = '{cellId}'");
            }


            return CellDependencies
                .FromSqlRaw(
                    @$"WITH RECURSIVE starting AS
                    (
                        SELECT sheet_id, depended_cell_id, depended_by_cell_id
                        FROM public.cell_dependencies
                        WHERE sheet_id = '{sheetId}' AND ({string.Join(" OR ", filters)})
                    UNION ALL
                        SELECT p.sheet_id, p.depended_cell_id, p.depended_by_cell_id
                        FROM starting AS s
                        JOIN public.cell_dependencies AS p ON s.depended_by_cell_id = p.depended_cell_id AND p.sheet_id = '{sheetId}'
                    )
                    SELECT DISTINCT *
                    FROM starting"
                )
                .Select(cd => cd.DependedByCell)
                .Concat(Cells.Where(c => c.SheetId == sheetId && dependedCellIds.Contains(c.CellId)))
                .Distinct(); 
        }

        public IQueryable<Cell> GetAllDependedBy(string sheetId, string dependedByCellId)  
        {
            return CellDependencies
                .FromSqlRaw(
                    @$"WITH RECURSIVE starting AS
                    (
                        SELECT sheet_id, depended_cell_id, depended_by_cell_id
                        FROM public.cell_dependencies
                        WHERE sheet_id = '{sheetId}' AND depended_by_cell_id = '{dependedByCellId}'
                    UNION ALL
                        SELECT p.sheet_id, p.depended_cell_id, p.depended_by_cell_id
                        FROM starting AS s
                        JOIN public.cell_dependencies AS p ON s.depended_cell_id = p.depended_by_cell_id AND p.sheet_id = '{sheetId}'
                    )
                    SELECT DISTINCT *
                    FROM starting"
                )
                .Select(cd => cd.DependedCell); 
        }
    }
}