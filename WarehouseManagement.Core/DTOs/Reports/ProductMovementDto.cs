using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Core.DTOs.Reports
{
    public class ProductMovementDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductSku { get; set; }
        public string CategoryName { get; set; }
        public int InboundQuantity { get; set; }
        public int OutboundQuantity { get; set; }
        public int NetQuantityChange { get; set; }
        public int TransactionCount { get; set; }
        public decimal InboundValue { get; set; }
        public decimal OutboundValue { get; set; }
        public decimal NetValueChange { get; set; }
        public DateTime? FirstTransactionDate { get; set; }
        public DateTime? LastTransactionDate { get; set; }
    }
}
