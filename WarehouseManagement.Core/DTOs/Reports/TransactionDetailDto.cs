using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseManagement.Core.Entities;

namespace WarehouseManagement.Core.DTOs.Reports
{
    public class TransactionDetailDto
    {
        public int TransactionId { get; set; }
        public DateTime TransactionDate { get; set; }
        public string ProductName { get; set; }
        public string ProductSku { get; set; }
        public string WarehouseName { get; set; }
        public TransactionType Type { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalValue { get; set; }
        public string Reference { get; set; }
        public string Description { get; set; }
        public string UserName { get; set; }
    }
}
