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
    public class CategoryRepository : BaseRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Category> GetCategoryWithProductsAsync(int id)
        {
            return await _entities
                .Include(c => c.Products)
                .ThenInclude(p => p.Supplier)
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
        }

        public async Task<Category> GetCategoryByNameAsync(string name)
        {
            return await _entities
                .FirstOrDefaultAsync(c => c.Name.ToLower() == name.ToLower() && !c.IsDeleted);
        }

        public async Task<bool> CategoryHasProductsAsync(int categoryId)
        {
            return await _entities
                .Include(c => c.Products)
                .AnyAsync(c => c.Id == categoryId && !c.IsDeleted && c.Products.Any(p => !p.IsDeleted));
        }

        public async Task<IEnumerable<Category>> GetCategoriesWithProductCountAsync()
        {
            return await _entities
                .Where(c => !c.IsDeleted)
                .Include(c => c.Products.Where(p => !p.IsDeleted))
                .Select(c => new Category
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    Products = c.Products,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt,
                    IsDeleted = c.IsDeleted
                })
                .ToListAsync();
        }

        public async Task<(IEnumerable<Category> Items, int TotalCount)> GetPagedCategoriesAsync(int pageNumber, int pageSize, string searchTerm = null)
        {
            var query = _entities.Where(c => !c.IsDeleted);

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(c =>
                    c.Name.Contains(searchTerm) ||
                    c.Description.Contains(searchTerm));
            }

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderBy(c => c.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }
    }
}
