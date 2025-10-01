using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Core.DTOs.Export
{
    public class ProductExportFilter
    {
        public int? CategoryId { get; set; }
        public int? SupplierId { get; set; }
        public bool? IncludeLowStock { get; set; }
        public bool? IncludeOutOfStock { get; set; }
        public string SearchTerm { get; set; }
        public DateTime? CreatedAfter { get; set; }
        public DateTime? CreatedBefore { get; set; }
    }
}
