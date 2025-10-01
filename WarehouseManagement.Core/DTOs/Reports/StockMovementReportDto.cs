using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Core.DTOs.Reports
{
    public class StockMovementReportDto
    {
        public string ReportTitle { get; set; } = "Stock Movement Report";
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
        public StockMovementSummaryDto Summary { get; set; }
        public IEnumerable<ProductMovementDto> ProductMovements { get; set; } = new List<ProductMovementDto>();
        public IEnumerable<WarehouseMovementDto> WarehouseMovements { get; set; } = new List<WarehouseMovementDto>();
        public IEnumerable<DailyMovementDto> DailyMovements { get; set; } = new List<DailyMovementDto>();
    }
}
