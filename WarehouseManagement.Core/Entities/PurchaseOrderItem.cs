using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Core.Entities
{
    public class PurchaseOrderItem : BaseEntity
    {
        public int PurchaseOrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        // Property با backing field
        private decimal _totalPrice;
        public decimal TotalPrice
        {
            get => _totalPrice != 0 ? _totalPrice : Quantity * UnitPrice;
            set => _totalPrice = value;
        }

        // Navigation properties
        public PurchaseOrder PurchaseOrder { get; set; }
        public Product Product { get; set; }
    }
}
