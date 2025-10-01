using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseManagement.Core.Entities;

namespace WarehouseManagement.Core.DTOs.FilterDtos
{
    public class TransactionReportFilter
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int? ProductId { get; set; }
        public int? WarehouseId { get; set; }
        public TransactionType? Type { get; set; }
    }
}
