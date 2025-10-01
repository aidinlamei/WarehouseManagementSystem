using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Core.DTOs.Products
{
    public class UpdateProductDto
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Cost { get; set; }

        [Range(0, int.MaxValue)]
        public int MinimumStockLevel { get; set; }

        [Range(1, int.MaxValue)]
        public int MaximumStockLevel { get; set; }
    }
}
