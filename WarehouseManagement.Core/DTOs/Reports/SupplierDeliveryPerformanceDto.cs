using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Core.DTOs.Reports
{
    public class SupplierDeliveryPerformanceDto
    {
        public int SupplierId { get; set; }
        public string SupplierName { get; set; }
        public int TotalOrders { get; set; }
        public int OnTimeOrders { get; set; }
        public int LateOrders { get; set; }
        public int EarlyOrders { get; set; }
        public decimal OnTimeDeliveryRate { get; set; }
        public int AverageDeliveryDays { get; set; }
        public int MinDeliveryDays { get; set; }
        public int MaxDeliveryDays { get; set; }
        public int MostFrequentDeliveryDays { get; set; }
    }
}
