using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Core.DTOs.Reports
{
    public class TransactionReportSummaryDto
    {
        public int TotalTransactions { get; set; }
        public int InboundCount { get; set; }
        public int OutboundCount { get; set; }
        public int AdjustmentCount { get; set; }
        public int TransferCount { get; set; }
        public int TotalInboundQuantity { get; set; }
        public int TotalOutboundQuantity { get; set; }
        public int NetQuantityChange { get; set; }
        public decimal TotalInboundValue { get; set; }
        public decimal TotalOutboundValue { get; set; }
        public decimal NetValueChange { get; set; }
        public int BusiestDayTransactions { get; set; }
        public DateTime BusiestDay { get; set; }
        public int MostActiveProductTransactions { get; set; }
        public string MostActiveProductName { get; set; }
    }
}
