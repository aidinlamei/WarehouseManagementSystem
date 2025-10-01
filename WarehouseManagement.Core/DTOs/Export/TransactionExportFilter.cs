using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseManagement.Core.Entities;

namespace WarehouseManagement.Core.DTOs.Export
{
    public class TransactionExportFilter
    {
        public int? ProductId { get; set; }
        public int? WarehouseId { get; set; }
        public TransactionType? Type { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Reference { get; set; }
    }
}
