using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Core.DTOs.Reports
{
    public class SalesSummaryDto
    {
        public int TotalTransactions { get; set; }
        public int TotalItemsSold { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal AverageSaleValue { get; set; }
        public decimal TotalCost { get; set; }
        public decimal TotalProfit { get; set; }
        public decimal ProfitMargin { get; set; }
        public int UniqueCustomers { get; set; }
        public int TopSellingProductId { get; set; }
        public string TopSellingProductName { get; set; }
        public int TopSellingProductQuantity { get; set; }
    }
}
