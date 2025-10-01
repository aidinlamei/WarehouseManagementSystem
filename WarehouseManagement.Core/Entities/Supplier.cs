using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Core.Entities
{
    public class Supplier : BaseEntity
    {
        public string Name { get; set; }
        public string ContactPerson { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }

        // Navigation properties
        public ICollection<Product> Products { get; set; }
        public ICollection<PurchaseOrder> PurchaseOrders { get; set; }
    }
}
