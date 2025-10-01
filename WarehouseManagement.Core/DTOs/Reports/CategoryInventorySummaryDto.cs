using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Core.DTOs.Reports
{
    public class CategoryInventorySummaryDto
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int ProductCount { get; set; }
        public int TotalQuantity { get; set; }
        public decimal TotalValue { get; set; }
        public decimal AveragePrice { get; set; }
        public int LowStockItems { get; set; }
        public int OutOfStockItems { get; set; }
    }
}
