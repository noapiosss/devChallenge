using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Contracts.Database
{
    [Table("cell_dependencies", Schema = "public")]
    public class CellDependency
    {
        [Column("depended_sheet_id")]
        public string DependedSheetId { get; set; }
        
        [Column("depended_cell_id")]
        public string DependedCellId { get; set;}
        public Cell DependedCell { get; set; }

        [Column("depended_by_sheet_id")]
        public string DependedBySheetId { get; set; }

        [Column("depended_by_cell_id")]
        public string DependedByCellId { get; set;}        
        public Cell DependedByCell { get; set; }
    }
}