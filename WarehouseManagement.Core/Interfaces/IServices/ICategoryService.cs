using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseManagement.Core.DTOs.Categories;

namespace WarehouseManagement.Core.Interfaces.IServices
{
    public interface ICategoryService
    {
        Task<CategoryDto> GetCategoryByIdAsync(int id);
        Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync();
        Task<(IEnumerable<CategoryDto> Items, int TotalCount)> GetPagedCategoriesAsync(int pageNumber, int pageSize, string searchTerm = null);
        Task CreateCategoryAsync(CreateCategoryDto categoryDto);
        Task UpdateCategoryAsync(int id, UpdateCategoryDto categoryDto);
        Task DeleteCategoryAsync(int id);
        Task<IEnumerable<CategoryWithProductsDto>> GetCategoriesWithProductsAsync();
        Task<bool> CategoryNameExistsAsync(string name, int? excludeCategoryId = null);
    }
}
