using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Core.DTOs.Reports
{
    public class WarehouseUtilizationDto
    {
        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; }
        public string Location { get; set; }
        public decimal TotalCapacity { get; set; }
        public decimal UsedCapacity { get; set; }
        public decimal AvailableCapacity { get; set; }
        public decimal UtilizationRate { get; set; }
        public int ProductCount { get; set; }
        public int TotalQuantity { get; set; }
        public decimal TotalValue { get; set; }
        public int LowStockItems { get; set; }
        public int OutOfStockItems { get; set; }
        public bool IsFull => UtilizationRate >= 95;
        public bool IsCritical => UtilizationRate >= 80;
        public string Status { get; set; }
    }
}
