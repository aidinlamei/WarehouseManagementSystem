using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Core.DTOs.Reports
{
    public class ProductPerformanceReportDto
    {
        public string ReportTitle { get; set; } = "Product Performance Report";
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
        public ProductPerformanceSummaryDto Summary { get; set; }
        public IEnumerable<ProductPerformanceDto> ProductPerformances { get; set; } = new List<ProductPerformanceDto>();
        public IEnumerable<SlowMovingProductDto> SlowMovingProducts { get; set; } = new List<SlowMovingProductDto>();
        public IEnumerable<FastMovingProductDto> FastMovingProducts { get; set; } = new List<FastMovingProductDto>();
    }
}
