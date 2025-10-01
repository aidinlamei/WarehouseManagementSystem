using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Core.DTOs.Reports
{
    public class ProductPerformanceDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductSku { get; set; }
        public string CategoryName { get; set; }
        public string SupplierName { get; set; }
        public int QuantitySold { get; set; }
        public int QuantityInStock { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal TotalCost { get; set; }
        public decimal TotalProfit { get; set; }
        public decimal ProfitMargin { get; set; }
        public decimal AverageSalePrice { get; set; }
        public int TransactionCount { get; set; }
        public int ReturnCount { get; set; }
        public decimal ReturnRate { get; set; }
        public int StockTurnover { get; set; }
        public decimal GMROI { get; set; } // Gross Margin Return on Investment
        public string PerformanceRating { get; set; }
    }
}
