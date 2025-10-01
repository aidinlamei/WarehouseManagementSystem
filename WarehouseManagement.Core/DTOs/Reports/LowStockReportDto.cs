using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Core.DTOs.Reports
{
    public class LowStockReportDto
    {
        public string ReportTitle { get; set; } = "Low Stock Report";
        public int Threshold { get; set; } = 10;
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
        public LowStockSummaryDto Summary { get; set; }
        public IEnumerable<LowStockItemDetailDto> LowStockItems { get; set; } = new List<LowStockItemDetailDto>();
        public IEnumerable<OutOfStockItemDetailDto> OutOfStockItems { get; set; } = new List<OutOfStockItemDetailDto>();
        public IEnumerable<CriticalStockItemDto> CriticalStockItems { get; set; } = new List<CriticalStockItemDto>();
    }
}
