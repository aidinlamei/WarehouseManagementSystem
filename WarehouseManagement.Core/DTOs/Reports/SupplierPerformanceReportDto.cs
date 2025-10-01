using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Core.DTOs.Reports
{
    public class SupplierPerformanceReportDto
    {
        public string ReportTitle { get; set; } = "Supplier Performance Report";
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
        public SupplierPerformanceSummaryDto Summary { get; set; }
        public IEnumerable<SupplierPerformanceDto> SupplierPerformances { get; set; } = new List<SupplierPerformanceDto>();
        public IEnumerable<SupplierDeliveryPerformanceDto> DeliveryPerformances { get; set; } = new List<SupplierDeliveryPerformanceDto>();
    }
}
