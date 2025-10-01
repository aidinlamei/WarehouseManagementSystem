using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Core.DTOs.Reports
{
    public class StockMovementSummaryDto
    {
        public int TotalTransactions { get; set; }
        public int TotalInboundQuantity { get; set; }
        public int TotalOutboundQuantity { get; set; }
        public int NetQuantityChange { get; set; }
        public decimal TotalInboundValue { get; set; }
        public decimal TotalOutboundValue { get; set; }
        public decimal NetValueChange { get; set; }
        public int MostActiveProductId { get; set; }
        public string MostActiveProductName { get; set; }
        public int MostActiveProductTransactions { get; set; }
        public int BusiestWarehouseId { get; set; }
        public string BusiestWarehouseName { get; set; }
        public int BusiestWarehouseTransactions { get; set; }
    }
}
