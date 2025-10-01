using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Core.DTOs.Reports
{
    public class SalesReportDto
    {
        public string ReportTitle { get; set; } = "Sales Report";
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
        public SalesSummaryDto Summary { get; set; }
        public IEnumerable<SalesByProductDto> SalesByProduct { get; set; } = new List<SalesByProductDto>();
        public IEnumerable<SalesByCategoryDto> SalesByCategory { get; set; } = new List<SalesByCategoryDto>();
        public IEnumerable<DailySalesDto> DailySales { get; set; } = new List<DailySalesDto>();
        public IEnumerable<MonthlySalesDto> MonthlySales { get; set; } = new List<MonthlySalesDto>();
    }
}
