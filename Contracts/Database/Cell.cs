using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Contracts.Database
{
    [Table("cells", Schema = "public")]
    public class Cell
    {
        [Column("sheet_id")]
        public string SheetId { get; set; }
        
        [Column("cell_id")]
        public string CellId { get; set;}

        [Column("value")]
        public string Value { get; set;}

        [Column("is_expression")]
        public bool IsExpression { get; set; }

        public ICollection<CellDependency> DependByCells { get; set; }
        public ICollection<CellDependency> DependedCells { get; set; }


        public Cell(string sheetId, string cellId, string value)
        {            
            SheetId = sheetId;
            CellId = cellId;

            if (value.StartsWith('='))
            {
                Value = value[1..];
                IsExpression = true;
            }
            else
            {
                Value = value;
                IsExpression = false;
            }
        }

        public override int GetHashCode()
        {
            return CellId.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is Cell other)
            {
                return SheetId == other.SheetId && CellId == other.CellId;
            }
            return false;
        }
    }
}