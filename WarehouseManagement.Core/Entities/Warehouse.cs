using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Core.Entities
{
    public class Warehouse : BaseEntity
    {
        public string Name { get; set; }
        public string Location { get; set; }
        public decimal Capacity { get; set; }
        public ICollection<Inventory> Inventories { get; set; }
        public ICollection<Transaction> Transactions { get; set; } // Added this property to fix the error
    }
}
