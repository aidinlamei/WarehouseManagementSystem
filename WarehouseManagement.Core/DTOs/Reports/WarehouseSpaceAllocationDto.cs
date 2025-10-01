using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Core.DTOs.Reports
{
    public class WarehouseSpaceAllocationDto
    {
        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public decimal SpaceUsed { get; set; }
        public decimal SpacePercentage { get; set; }
        public int ProductCount { get; set; }
        public int TotalQuantity { get; set; }
        public decimal TotalValue { get; set; }
    }
}
