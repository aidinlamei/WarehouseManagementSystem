using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Core.DTOs.Inventory
{
    public class InventorySummaryDto
    {
        public int TotalProducts { get; set; }
        public int TotalWarehouses { get; set; }
        public int TotalInventoryItems { get; set; }
        public int TotalQuantity { get; set; }
        public int TotalReservedQuantity { get; set; }
        public int TotalAvailableQuantity { get; set; }
        public decimal TotalValue { get; set; }
        public int LowStockCount { get; set; }
        public int OutOfStockCount { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
