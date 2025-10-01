using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Core.DTOs.Warehouses
{
    public class WarehouseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public decimal Capacity { get; set; }
        public decimal UsedCapacity { get; set; }
        public decimal AvailableCapacity { get; set; }
        public decimal UtilizationPercentage { get; set; }
        public int ProductCount { get; set; }
        public int TotalItems { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
