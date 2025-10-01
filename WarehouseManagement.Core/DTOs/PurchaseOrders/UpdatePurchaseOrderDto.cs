using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Core.DTOs.PurchaseOrders
{
    public class UpdatePurchaseOrderDto
    {
        [Required]
        public int SupplierId { get; set; }

        public DateTime? ExpectedDeliveryDate { get; set; }

        [StringLength(1000)]
        public string Notes { get; set; }
    }
}
