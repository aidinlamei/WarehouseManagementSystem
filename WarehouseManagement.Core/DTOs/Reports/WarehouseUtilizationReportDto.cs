using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Core.DTOs.Reports
{
    public class WarehouseUtilizationReportDto
    {
        public string ReportTitle { get; set; } = "Warehouse Utilization Report";
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
        public WarehouseUtilizationSummaryDto Summary { get; set; }
        public IEnumerable<WarehouseUtilizationDto> WarehouseUtilizations { get; set; } = new List<WarehouseUtilizationDto>();
        public IEnumerable<WarehouseSpaceAllocationDto> SpaceAllocations { get; set; } = new List<WarehouseSpaceAllocationDto>();
    }
}
