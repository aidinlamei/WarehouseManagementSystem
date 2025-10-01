using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseManagement.Core.Entities;

namespace WarehouseManagement.Core.Interfaces
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task<Category> GetCategoryWithProductsAsync(int id);
        Task<Category> GetCategoryByNameAsync(string name);
        Task<bool> CategoryHasProductsAsync(int categoryId);
        Task<IEnumerable<Category>> GetCategoriesWithProductCountAsync();
        Task<(IEnumerable<Category> Items, int TotalCount)> GetPagedCategoriesAsync(int pageNumber, int pageSize, string searchTerm = null);
    }
}
