using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Core.DTOs.PurchaseOrders
{
    public class PurchaseOrderSummaryDto
    {
        public int TotalOrders { get; set; }
        public int DraftCount { get; set; }
        public int PendingCount { get; set; }
        public int ApprovedCount { get; set; }
        public int CompletedCount { get; set; }
        public int CancelledCount { get; set; }
        public decimal TotalOrderValue { get; set; }
        public decimal AverageOrderValue { get; set; }
        public int PendingItemsCount { get; set; }
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    }
}
