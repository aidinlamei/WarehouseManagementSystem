using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Core.DTOs.Reports
{
    public class LowStockSummaryDto
    {
        public int TotalProducts { get; set; }
        public int LowStockCount { get; set; }
        public int OutOfStockCount { get; set; }
        public int CriticalStockCount { get; set; }
        public int BelowMinimumCount { get; set; }
        public decimal TotalValueAtRisk { get; set; }
        public int MostAffectedCategoryId { get; set; }
        public string MostAffectedCategoryName { get; set; }
        public int MostAffectedSupplierId { get; set; }
        public string MostAffectedSupplierName { get; set; }
    }
}
