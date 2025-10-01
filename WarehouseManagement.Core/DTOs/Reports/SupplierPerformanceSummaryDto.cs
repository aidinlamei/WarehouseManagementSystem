using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Core.DTOs.Reports
{
    public class SupplierPerformanceSummaryDto
    {
        public int TotalSuppliers { get; set; }
        public int ActiveSuppliers { get; set; }
        public int TotalPurchaseOrders { get; set; }
        public decimal TotalOrderValue { get; set; }
        public decimal AverageOrderValue { get; set; }
        public int TopSupplierId { get; set; }
        public string TopSupplierName { get; set; }
        public decimal TopSupplierTotalValue { get; set; }
        public decimal OnTimeDeliveryRate { get; set; }
        public decimal QualityRating { get; set; }
    }
}
