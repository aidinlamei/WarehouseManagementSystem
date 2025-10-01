using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace WarehouseManagement.Core.Entities
{
    public class Product : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string SKU { get; set; }
        public decimal Price { get; set; }
        public decimal Cost { get; set; }
        public int CategoryId { get; set; }
        public int SupplierId { get; set; }
        public int MinimumStockLevel { get; set; }
        public int MaximumStockLevel { get; set; }

        // Navigation properties
        public Category Category { get; set; }
        public Supplier Supplier { get; set; }
        public ICollection<Inventory> Inventories { get; set; }
        public ICollection<Transaction> Transactions { get; set; }
    }
}
