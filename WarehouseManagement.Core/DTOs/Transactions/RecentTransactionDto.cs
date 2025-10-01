using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseManagement.Core.Entities;

namespace WarehouseManagement.Core.DTOs.Transactions
{
    public class RecentTransactionDto
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public string WarehouseName { get; set; }
        public TransactionType Type { get; set; }
        public int Quantity { get; set; }
        public string Reference { get; set; }
        public DateTime CreatedAt { get; set; }
        public string TimeAgo { get; set; }
    }
}
