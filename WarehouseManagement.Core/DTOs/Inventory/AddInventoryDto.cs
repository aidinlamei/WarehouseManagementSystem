using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Core.DTOs.Inventory
{
    public class AddInventoryDto
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        public int WarehouseId { get; set; }

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        public string Reason { get; set; }
    }
}
