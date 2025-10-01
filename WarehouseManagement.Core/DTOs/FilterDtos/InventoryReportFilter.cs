using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Core.DTOs.FilterDtos
{
    public class InventoryReportFilter
    {
        public int? WarehouseId { get; set; }
        public int? CategoryId { get; set; }
        public int? SupplierId { get; set; }
        public bool? IncludeLowStock { get; set; }
        public bool? IncludeOutOfStock { get; set; }
    }
}
