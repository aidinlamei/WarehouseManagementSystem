using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Core.DTOs.Reports
{
    public class DailyMovementDto
    {
        public DateTime Date { get; set; }
        public int InboundTransactions { get; set; }
        public int OutboundTransactions { get; set; }
        public int TotalTransactions { get; set; }
        public int InboundQuantity { get; set; }
        public int OutboundQuantity { get; set; }
        public int NetQuantityChange { get; set; }
        public decimal InboundValue { get; set; }
        public decimal OutboundValue { get; set; }
        public decimal NetValueChange { get; set; }
    }
}
