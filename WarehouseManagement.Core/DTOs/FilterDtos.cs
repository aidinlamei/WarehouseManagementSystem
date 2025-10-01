using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseManagement.Core.Entities;

namespace WarehouseManagement.Core.DTOs
{
    public class TransactionFilter
    {
        public int? ProductId { get; set; }
        public int? WarehouseId { get; set; }
        public TransactionType? Type { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Reference { get; set; }
    }

    public class PurchaseOrderFilter
    {
        public int? SupplierId { get; set; }
        public OrderStatus? Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string OrderNumber { get; set; }
    }

    public class InventoryReportFilter
    {
        public int? WarehouseId { get; set; }
        public int? CategoryId { get; set; }
        public int? SupplierId { get; set; }
        public bool? IncludeLowStock { get; set; }
        public bool? IncludeOutOfStock { get; set; }
    }

    public class TransactionReportFilter
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int? ProductId { get; set; }
        public int? WarehouseId { get; set; }
        public TransactionType? Type { get; set; }
    }
}
