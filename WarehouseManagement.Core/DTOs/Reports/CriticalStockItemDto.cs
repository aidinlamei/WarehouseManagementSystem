using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Core.DTOs.Reports
{
    public class CriticalStockItemDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductSku { get; set; }
        public string CategoryName { get; set; }
        public string SupplierName { get; set; }
        public int CurrentStock { get; set; }
        public int MinimumStockLevel { get; set; }
        public int DaysOfSupply { get; set; }
        public decimal DailySalesRate { get; set; }
        public int ExpectedStockOutDays { get; set; }
        public string CriticalityLevel { get; set; }
        public DateTime ProjectedStockOutDate { get; set; }
        public decimal PotentialLostSales { get; set; }
    }
}
