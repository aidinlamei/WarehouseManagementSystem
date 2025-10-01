using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseManagement.Core.Entities;

namespace WarehouseManagement.Core.DTOs.Reports
{
    public class TransactionTypeSummaryDto
    {
        public TransactionType Type { get; set; }
        public int Count { get; set; }
        public int TotalQuantity { get; set; }
        public decimal TotalValue { get; set; }
        public decimal PercentageOfTotal { get; set; }
        public decimal AverageQuantity { get; set; }
        public decimal AverageValue { get; set; }
    }
}
