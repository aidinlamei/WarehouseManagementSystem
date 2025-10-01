using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Core.DTOs.Inventory
{
    public class TransferInventoryDto
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        public int FromWarehouseId { get; set; }

        [Required]
        public int ToWarehouseId { get; set; }

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        public string Reason { get; set; }
    }
}
