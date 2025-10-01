using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Core.DTOs.Reports
{
    public class WarehouseUtilizationSummaryDto
    {
        public int TotalWarehouses { get; set; }
        public int ActiveWarehouses { get; set; }
        public decimal TotalCapacity { get; set; }
        public decimal TotalUsedCapacity { get; set; }
        public decimal TotalAvailableCapacity { get; set; }
        public decimal OverallUtilizationRate { get; set; }
        public int MostUtilizedWarehouseId { get; set; }
        public string MostUtilizedWarehouseName { get; set; }
        public decimal MostUtilizedWarehouseRate { get; set; }
        public int LeastUtilizedWarehouseId { get; set; }
        public string LeastUtilizedWarehouseName { get; set; }
        public decimal LeastUtilizedWarehouseRate { get; set; }
    }
}
