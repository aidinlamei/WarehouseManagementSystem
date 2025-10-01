using WarehouseManagement.Core.Entities;

namespace WarehouseManagement.Core.Interfaces
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<Product> GetProductWithDetailsAsync(int id);
        Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId);
        Task<IEnumerable<Product>> GetLowStockProductsAsync(int threshold = 10);
        Task<Product> GetProductBySkuAsync(string sku);
        Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm);

        // تغییر این متد به بازگشت tuple
        Task<(IEnumerable<Product> Items, int TotalCount)> GetPagedProductsAsync(int pageNumber, int pageSize, string searchTerm = null);
    }
}