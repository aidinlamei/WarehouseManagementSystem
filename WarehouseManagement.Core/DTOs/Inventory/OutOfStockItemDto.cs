using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Core.DTOs.Inventory
{
    public class OutOfStockItemDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductSku { get; set; }
        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; }
        public DateTime LastStockDate { get; set; }
    }
}
