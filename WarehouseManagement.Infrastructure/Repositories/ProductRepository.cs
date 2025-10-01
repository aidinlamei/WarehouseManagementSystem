using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseManagement.Core.Entities;
using WarehouseManagement.Core.Interfaces;
using WarehouseManagement.Infrastructure.Data;

namespace WarehouseManagement.Infrastructure.Repositories
{
    public class ProductRepository : BaseRepository<Product>, IProductRepository
    {
        public ProductRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Product> GetProductWithDetailsAsync(int id)
        {
            return await _entities
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .Include(p => p.Inventories)
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
        }

        public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId)
        {
            return await _entities
                .Where(p => p.CategoryId == categoryId && !p.IsDeleted)
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetLowStockProductsAsync(int threshold = 10)
        {
            return await _entities
                .Where(p => !p.IsDeleted)
                .Include(p => p.Inventories)
                .Where(p => p.Inventories.Sum(i => i.Quantity) <= threshold)
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .ToListAsync();
        }

        public async Task<Product> GetProductBySkuAsync(string sku)
        {
            return await _entities
                .FirstOrDefaultAsync(p => p.SKU == sku && !p.IsDeleted);
        }

        public async Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm)
        {
            return await _entities
                .Where(p => !p.IsDeleted &&
                           (p.Name.Contains(searchTerm) ||
                            p.Description.Contains(searchTerm) ||
                            p.SKU.Contains(searchTerm)))
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .ToListAsync();
        }

        // تغییر این متد به بازگشت tuple
        public async Task<(IEnumerable<Product> Items, int TotalCount)> GetPagedProductsAsync(int pageNumber, int pageSize, string searchTerm = null)
        {
            var query = _entities.Where(p => !p.IsDeleted);

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(p =>
                    p.Name.Contains(searchTerm) ||
                    p.Description.Contains(searchTerm) ||
                    p.SKU.Contains(searchTerm));
            }

            query = query
                .Include(p => p.Category)
                .Include(p => p.Supplier);

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderBy(p => p.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }
    }
}
