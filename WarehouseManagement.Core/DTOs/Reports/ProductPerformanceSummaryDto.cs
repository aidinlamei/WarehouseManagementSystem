using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Core.DTOs.Reports
{
    public class ProductPerformanceSummaryDto
    {
        public int TotalProducts { get; set; }
        public int ActiveProducts { get; set; }
        public int ProductsWithSales { get; set; }
        public int ProductsWithoutSales { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal TotalProfit { get; set; }
        public decimal AverageProfitMargin { get; set; }
        public int TopPerformingProductId { get; set; }
        public string TopPerformingProductName { get; set; }
        public decimal TopPerformingProductRevenue { get; set; }
        public int WorstPerformingProductId { get; set; }
        public string WorstPerformingProductName { get; set; }
        public decimal WorstPerformingProductRevenue { get; set; }
    }
}
