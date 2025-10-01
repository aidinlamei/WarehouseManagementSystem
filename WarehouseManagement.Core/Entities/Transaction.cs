using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Core.Entities
{
    public class Transaction : BaseEntity
    {
        public int ProductId { get; set; }
        public int WarehouseId { get; set; }
        public TransactionType Type { get; set; }
        public int Quantity { get; set; }
        public string Reference { get; set; } // شماره فاکتور یا سند
        public string Description { get; set; }

        // Navigation properties
        public Product Product { get; set; }
        public Warehouse Warehouse { get; set; }
    }
}
