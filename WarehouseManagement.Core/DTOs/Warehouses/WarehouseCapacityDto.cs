using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Core.DTOs.Warehouses
{
    public class WarehouseCapacityDto
    {
        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; }
        public decimal TotalCapacity { get; set; }
        public decimal UsedCapacity { get; set; }
        public decimal AvailableCapacity { get; set; }
        public decimal UtilizationPercentage { get; set; }
        public bool IsFull { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
