using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Core.Entities
{
    public class PurchaseOrder : BaseEntity
    {
        public string OrderNumber { get; set; }
        public int SupplierId { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? ExpectedDeliveryDate { get; set; }
        public OrderStatus Status { get; set; }
        public decimal TotalAmount { get; set; }

        // Navigation properties
        public Supplier Supplier { get; set; }
        public ICollection<PurchaseOrderItem> Items { get; set; }
    }
}
