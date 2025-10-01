using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Core.DTOs.Export
{
    public class InventoryExportFilter
    {
        public int? ProductId { get; set; }
        public int? WarehouseId { get; set; }
        public int? CategoryId { get; set; }
        public bool? LowStockOnly { get; set; }
        public bool? OutOfStockOnly { get; set; }
        public DateTime? UpdatedAfter { get; set; }
        public DateTime? UpdatedBefore { get; set; }
    }
}
