using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Core.DTOs.Reports
{
    public class FastMovingProductDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductSku { get; set; }
        public string CategoryName { get; set; }
        public int QuantitySold { get; set; }
        public int QuantityInStock { get; set; }
        public decimal StockValue { get; set; }
        public int DaysInStock { get; set; }
        public decimal SalesVelocity { get; set; }
        public int StockOutCount { get; set; }
        public string Status { get; set; }
        public DateTime? LastRestockDate { get; set; }
    }
}
