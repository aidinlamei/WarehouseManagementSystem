using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseManagement.Core.DTOs.Categories;
using WarehouseManagement.Core.DTOs.Products;
using WarehouseManagement.Core.Entities;
using WarehouseManagement.Core.Exceptions;
using WarehouseManagement.Core.Interfaces;
using WarehouseManagement.Core.Interfaces.IServices;

namespace WarehouseManagement.Infrastructure.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly IAppLogger<CategoryService> _logger;

        public CategoryService(
            ICategoryRepository categoryRepository,
            IProductRepository productRepository,
            IMapper mapper,
            IAppLogger<CategoryService> logger)
        {
            _categoryRepository = categoryRepository;
            _productRepository = productRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<CategoryDto> GetCategoryByIdAsync(int id)
        {
            _logger.LogInformation("Getting category by ID: {CategoryId}", id);

            var category = await _categoryRepository.GetCategoryWithProductsAsync(id);
            if (category == null)
            {
                _logger.LogWarning("Category with ID {CategoryId} not found", id);
                throw new NotFoundException("Category", id, ErrorCodes.NOT_FOUND);
            }

            var categoryDto = _mapper.Map<CategoryDto>(category);
            categoryDto.ProductCount = category.Products?.Count(p => !p.IsDeleted) ?? 0;

            _logger.LogInformation("Successfully retrieved category with ID: {CategoryId}", id);
            return categoryDto;
        }

        public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
        {
            _logger.LogInformation("Getting all categories");

            var categories = await _categoryRepository.GetAllAsync();
            var categoryDtos = _mapper.Map<IEnumerable<CategoryDto>>(categories);

            // محاسبه تعداد محصولات برای هر دسته‌بندی
            foreach (var categoryDto in categoryDtos)
            {
                var products = await _productRepository.GetProductsByCategoryAsync(categoryDto.Id);
                categoryDto.ProductCount = products.Count();
            }

            _logger.LogInformation("Successfully retrieved {CategoryCount} categories", categoryDtos.Count());
            return categoryDtos;
        }

        public async Task<(IEnumerable<CategoryDto> Items, int TotalCount)> GetPagedCategoriesAsync(int pageNumber, int pageSize, string searchTerm = null)
        {
            _logger.LogInformation("Getting paged categories - Page: {PageNumber}, Size: {PageSize}, Search: {SearchTerm}",
                pageNumber, pageSize, searchTerm);

            if (pageNumber < 1)
                throw new ValidationException("Page number must be greater than 0");

            if (pageSize < 1 || pageSize > 100)
                throw new ValidationException("Page size must be between 1 and 100");

            // استفاده از CategoryRepository برای دریافت داده‌های صفحه‌بندی شده
            var (categories, totalCount) = await _categoryRepository.GetPagedCategoriesAsync(pageNumber, pageSize, searchTerm);
            var categoryDtos = _mapper.Map<IEnumerable<CategoryDto>>(categories);

            // محاسبه تعداد محصولات برای هر دسته‌بندی
            foreach (var categoryDto in categoryDtos)
            {
                var products = await _productRepository.GetProductsByCategoryAsync(categoryDto.Id);
                categoryDto.ProductCount = products.Count();
            }

            _logger.LogInformation("Successfully retrieved {CategoryCount} categories (page {PageNumber})",
                categoryDtos.Count(), pageNumber);

            return (categoryDtos, totalCount);
        }
        public async Task CreateCategoryAsync(CreateCategoryDto categoryDto)
        {
            _logger.LogInformation("Creating new category with name: {CategoryName}", categoryDto.Name);

            // اعتبارسنجی نام تکراری
            var existingCategory = await _categoryRepository.GetCategoryByNameAsync(categoryDto.Name);
            if (existingCategory != null)
            {
                _logger.LogWarning("Category with name {CategoryName} already exists", categoryDto.Name);
                throw new BusinessException(
                    "Category with this name already exists",
                    "CATEGORY_NAME_UNIQUE",
                    ErrorCodes.BUSINESS_RULE_VIOLATION
                );
            }

            try
            {
                var category = _mapper.Map<Category>(categoryDto);
                await _categoryRepository.AddAsync(category);
                await _categoryRepository.SaveChangesAsync();

                _logger.LogInformation("Successfully created category with ID: {CategoryId} and name: {CategoryName}",
                    category.Id, category.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating category with name: {CategoryName}", categoryDto.Name);
                throw new BusinessException(
                    "An error occurred while creating the category",
                    "CATEGORY_CREATE_ERROR",
                    ErrorCodes.UNEXPECTED_ERROR,
                    ex
                );
            }
        }

        public async Task UpdateCategoryAsync(int id, UpdateCategoryDto categoryDto)
        {
            _logger.LogInformation("Updating category with ID: {CategoryId}", id);

            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                _logger.LogWarning("Category with ID {CategoryId} not found for update", id);
                throw new NotFoundException("Category", id, ErrorCodes.NOT_FOUND);
            }

            // اعتبارسنجی نام تکراری
            var existingCategory = await _categoryRepository.GetCategoryByNameAsync(categoryDto.Name);
            if (existingCategory != null && existingCategory.Id != id)
            {
                _logger.LogWarning("Category with name {CategoryName} already exists", categoryDto.Name);
                throw new BusinessException(
                    "Category with this name already exists",
                    "CATEGORY_NAME_UNIQUE",
                    ErrorCodes.BUSINESS_RULE_VIOLATION
                );
            }

            try
            {
                category.Name = categoryDto.Name;
                category.Description = categoryDto.Description;
                category.UpdatedAt = DateTime.UtcNow;

                _categoryRepository.Update(category);
                await _categoryRepository.SaveChangesAsync();

                _logger.LogInformation("Successfully updated category with ID: {CategoryId}", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating category with ID: {CategoryId}", id);
                throw new BusinessException(
                    "An error occurred while updating the category",
                    "CATEGORY_UPDATE_ERROR",
                    ErrorCodes.UNEXPECTED_ERROR,
                    ex
                );
            }
        }

        public async Task DeleteCategoryAsync(int id)
        {
            _logger.LogInformation("Deleting category with ID: {CategoryId}", id);

            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                _logger.LogWarning("Category with ID {CategoryId} not found for deletion", id);
                throw new NotFoundException("Category", id, ErrorCodes.NOT_FOUND);
            }

            try
            {
                // بررسی آیا دسته‌بندی دارای محصول است
                var hasProducts = await _categoryRepository.CategoryHasProductsAsync(id);
                if (hasProducts)
                {
                    _logger.LogWarning("Cannot delete category with ID {CategoryId} because it has products", id);
                    throw new BusinessException(
                        "Cannot delete category that has products. Please remove or reassign products first.",
                        "CATEGORY_HAS_PRODUCTS",
                        ErrorCodes.CATEGORY_HAS_PRODUCTS
                    );
                }

                _categoryRepository.Delete(category);
                await _categoryRepository.SaveChangesAsync();

                _logger.LogInformation("Successfully deleted category with ID: {CategoryId}", id);
            }
            catch (BusinessException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting category with ID: {CategoryId}", id);
                throw new BusinessException(
                    "An error occurred while deleting the category",
                    "CATEGORY_DELETE_ERROR",
                    ErrorCodes.UNEXPECTED_ERROR,
                    ex
                );
            }
        }

        public async Task<IEnumerable<CategoryWithProductsDto>> GetCategoriesWithProductsAsync()
        {
            _logger.LogInformation("Getting categories with products");

            var categories = await _categoryRepository.GetCategoriesWithProductCountAsync();
            var categoryDtos = _mapper.Map<IEnumerable<CategoryWithProductsDto>>(categories);

            foreach (var categoryDto in categoryDtos)
            {
                var products = await _productRepository.GetProductsByCategoryAsync(categoryDto.Id);
                categoryDto.Products = _mapper.Map<IEnumerable<ProductDto>>(products);

                // محاسبه موجودی برای هر محصول
                foreach (var product in categoryDto.Products)
                {
                    product.TotalQuantity = await CalculateProductTotalQuantityAsync(product.Id);
                }
            }

            _logger.LogInformation("Successfully retrieved {CategoryCount} categories with products", categoryDtos.Count());
            return categoryDtos;
        }

        public async Task<bool> CategoryNameExistsAsync(string name, int? excludeCategoryId = null)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ValidationException("Category name cannot be empty");
            }

            var category = await _categoryRepository.GetCategoryByNameAsync(name);
            return category == null || category.Id == excludeCategoryId;
        }

        private async Task<int> CalculateProductTotalQuantityAsync(int productId)
        {
            try
            {
                var inventories = await GetProductInventoriesAsync(productId);
                return inventories.Sum(i => i.Quantity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating total quantity for product ID: {ProductId}", productId);
                return 0;
            }
        }

        private async Task<IEnumerable<Inventory>> GetProductInventoriesAsync(int productId)
        {
            // این متد باید از InventoryRepository استفاده کند
            // برای حالا یک پیاده‌سازی ساده
            return new List<Inventory>();
        }
    }
}
