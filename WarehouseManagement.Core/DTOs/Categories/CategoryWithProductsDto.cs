using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseManagement.Core.DTOs.Products;

namespace WarehouseManagement.Core.DTOs.Categories
{
    public class CategoryWithProductsDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public IEnumerable<ProductDto> Products { get; set; } = new List<ProductDto>();
        public int ProductCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
