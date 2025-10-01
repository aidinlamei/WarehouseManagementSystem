using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Core.DTOs.Transactions
{
    public class TransactionSummaryDto
    {
        public int TotalTransactions { get; set; }
        public int InboundCount { get; set; }
        public int OutboundCount { get; set; }
        public int AdjustmentCount { get; set; }
        public int TransferCount { get; set; }
        public int TotalInboundQuantity { get; set; }
        public int TotalOutboundQuantity { get; set; }
        public int NetQuantityChange { get; set; }
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    }
}
