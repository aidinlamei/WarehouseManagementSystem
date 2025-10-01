using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Core.DTOs.PurchaseOrders
{
    public class CreatePurchaseOrderDto
    {
        [Required]
        [StringLength(50)]
        public string OrderNumber { get; set; }

        [Required]
        public int SupplierId { get; set; }

        [Required]
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        public DateTime? ExpectedDeliveryDate { get; set; }

        [Required]
        public List<CreatePurchaseOrderItemDto> Items { get; set; } = new List<CreatePurchaseOrderItemDto>();

        [StringLength(1000)]
        public string Notes { get; set; }
    }
}
