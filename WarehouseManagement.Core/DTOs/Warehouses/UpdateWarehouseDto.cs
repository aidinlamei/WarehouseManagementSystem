using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Core.DTOs.Warehouses
{
    public class UpdateWarehouseDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(300)]
        public string Location { get; set; }

        [Range(1, double.MaxValue)]
        public decimal Capacity { get; set; }
    }
}
