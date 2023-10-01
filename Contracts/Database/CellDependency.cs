using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Contracts.Database
{
    [Table("cell_dependencies", Schema = "public")]
    public class CellDependency
    {
        [Column("sheet_id")]
        public string SheetId { get; set; }
        
        [Column("depended_cell_id")]
        public string DependedCellId { get; set;}
        public Cell DependedCell { get; set; }

        [Column("depended_by_cell_id")]
        public string DependedByCellId { get; set;}        
        public Cell DependedByCell { get; set; }
    }
}