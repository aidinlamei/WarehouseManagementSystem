using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Core.DTOs.Reports
{
    public class WarehouseMovementDto
    {
        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; }
        public int InboundQuantity { get; set; }
        public int OutboundQuantity { get; set; }
        public int NetQuantityChange { get; set; }
        public int TransactionCount { get; set; }
        public decimal InboundValue { get; set; }
        public decimal OutboundValue { get; set; }
        public decimal NetValueChange { get; set; }
        public int MostTransferredProductId { get; set; }
        public string MostTransferredProductName { get; set; }
    }
}
