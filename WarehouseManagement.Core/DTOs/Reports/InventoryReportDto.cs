using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseManagement.Core.DTOs.Inventory;

namespace WarehouseManagement.Core.DTOs.Reports
{
    public class InventoryReportDto
    {
        public string ReportTitle { get; set; } = "Inventory Report";
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
        public InventoryReportFilter Filter { get; set; }
        public InventorySummaryDto Summary { get; set; }
        public IEnumerable<InventoryItemReportDto> Items { get; set; } = new List<InventoryItemReportDto>();
        public IEnumerable<WarehouseInventorySummaryDto> WarehouseSummaries { get; set; } = new List<WarehouseInventorySummaryDto>();
        public IEnumerable<CategoryInventorySummaryDto> CategorySummaries { get; set; } = new List<CategoryInventorySummaryDto>();
    }
}
