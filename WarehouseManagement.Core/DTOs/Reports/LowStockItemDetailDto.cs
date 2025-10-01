using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Core.DTOs.Reports
{
    public class LowStockItemDetailDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductSku { get; set; }
        public string CategoryName { get; set; }
        public string SupplierName { get; set; }
        public int CurrentStock { get; set; }
        public int MinimumStockLevel { get; set; }
        public int MaximumStockLevel { get; set; }
        public int RequiredReorderQuantity { get; set; }
        public decimal ProductPrice { get; set; }
        public decimal StockValue { get; set; }
        public int DaysOfSupply { get; set; }
        public string UrgencyLevel { get; set; }
        public DateTime? LastRestockDate { get; set; }
        public DateTime? ExpectedRestockDate { get; set; }
    }
}
