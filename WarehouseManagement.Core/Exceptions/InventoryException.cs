using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Core.Exceptions
{
    public class InventoryException : BusinessException
    {
        public int ProductId { get; set; }
        public int WarehouseId { get; set; }
        public int RequestedQuantity { get; set; }
        public int AvailableQuantity { get; set; }

        public InventoryException(string message, int productId, int warehouseId, int requestedQuantity, int availableQuantity)
            : base(message, "INVENTORY_ERROR")
        {
            ProductId = productId;
            WarehouseId = warehouseId;
            RequestedQuantity = requestedQuantity;
            AvailableQuantity = availableQuantity;
        }

        public InventoryException(string message, int productId, int warehouseId, int requestedQuantity, int availableQuantity, Exception innerException)
            : base(message, "INVENTORY_ERROR", innerException)
        {
            ProductId = productId;
            WarehouseId = warehouseId;
            RequestedQuantity = requestedQuantity;
            AvailableQuantity = availableQuantity;
        }
    }
}
