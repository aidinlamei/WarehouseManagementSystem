using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Core.DTOs.Reports
{
    public class TransactionTrendDto
    {
        public DateTime Period { get; set; }
        public string PeriodLabel { get; set; }
        public int InboundCount { get; set; }
        public int OutboundCount { get; set; }
        public int TotalCount { get; set; }
        public int InboundQuantity { get; set; }
        public int OutboundQuantity { get; set; }
        public int NetQuantity { get; set; }
        public decimal InboundValue { get; set; }
        public decimal OutboundValue { get; set; }
        public decimal NetValue { get; set; }
    }
}
