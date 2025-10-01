using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseManagement.Core.DTOs;
using WarehouseManagement.Core.DTOs.Products;
using WarehouseManagement.Core.Entities;

namespace WarehouseManagement.Core.Interfaces.IServices
{
    public interface IProductService
    {
        Task<ProductDto> GetProductByIdAsync(int id);
        Task<IEnumerable<ProductDto>> GetAllProductsAsync();

        // تغییر این متد به بازگشت tuple
        Task<(IEnumerable<ProductDto> Items, int TotalCount)> GetPagedProductsAsync(int pageNumber, int pageSize, string searchTerm = null);

        Task CreateProductAsync(CreateProductDto productDto);
        Task UpdateProductAsync(int id, UpdateProductDto productDto);
        Task DeleteProductAsync(int id);
        Task<IEnumerable<ProductDto>> GetLowStockProductsAsync(int threshold = 10);
        Task<IEnumerable<ProductDto>> SearchProductsAsync(string searchTerm);
        Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(int categoryId);
        Task<bool> IsSkuUniqueAsync(string sku, int? excludeProductId = null);
        Task<ProductStatsDto> GetProductStatsAsync(int productId);
    }
}
