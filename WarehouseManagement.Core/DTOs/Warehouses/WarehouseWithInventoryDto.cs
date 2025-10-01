using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseManagement.Core.DTOs.Inventory;

namespace WarehouseManagement.Core.DTOs.Warehouses
{
    public class WarehouseWithInventoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public decimal Capacity { get; set; }
        public decimal UsedCapacity { get; set; }
        public decimal AvailableCapacity { get; set; }
        public decimal UtilizationPercentage { get; set; }
        public IEnumerable<InventoryDto> InventoryItems { get; set; } = new List<InventoryDto>();
        public int TotalProducts { get; set; }
        public int TotalQuantity { get; set; }
        public decimal TotalValue { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
