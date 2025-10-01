// WarehouseManagement.Infrastructure/Services/ProductService.cs
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WarehouseManagement.Core.DTOs;
using WarehouseManagement.Core.DTOs.Products;
using WarehouseManagement.Core.Entities;
using WarehouseManagement.Core.Exceptions;
using WarehouseManagement.Core.Interfaces;
using WarehouseManagement.Core.Interfaces.IServices;

namespace WarehouseManagement.Infrastructure.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ISupplierRepository _supplierRepository;
        private readonly IInventoryRepository _inventoryRepository;
        private readonly IMapper _mapper;
        private readonly IAppLogger<ProductService> _logger;

        public ProductService(
            IProductRepository productRepository,
            ICategoryRepository categoryRepository,
            ISupplierRepository supplierRepository,
            IInventoryRepository inventoryRepository,
            IMapper mapper,
            IAppLogger<ProductService> logger)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _supplierRepository = supplierRepository;
            _inventoryRepository = inventoryRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ProductDto> GetProductByIdAsync(int id)
        {
            _logger.LogInformation("Getting product by ID: {ProductId}", id);

            var product = await _productRepository.GetProductWithDetailsAsync(id);
            if (product == null)
            {
                _logger.LogWarning("Product with ID {ProductId} not found", id);
                throw new NotFoundException("Product", id, ErrorCodes.NOT_FOUND);
            }

            var productDto = _mapper.Map<ProductDto>(product);
            productDto.TotalQuantity = await CalculateTotalQuantityAsync(id);
            productDto.CategoryName = product.Category?.Name;
            productDto.SupplierName = product.Supplier?.Name;
            productDto.MinimumStockLevel = product.MinimumStockLevel;
            productDto.MaximumStockLevel = product.MaximumStockLevel;

            _logger.LogInformation("Successfully retrieved product with ID: {ProductId}", id);
            return productDto;
        }

        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
        {
            _logger.LogInformation("Getting all products");

            var products = await _productRepository.GetAllAsync();
            var productDtos = _mapper.Map<IEnumerable<ProductDto>>(products);

            foreach (var productDto in productDtos)
            {
                productDto.TotalQuantity = await CalculateTotalQuantityAsync(productDto.Id);
                var product = products.First(p => p.Id == productDto.Id);
                productDto.CategoryName = product.Category?.Name;
                productDto.SupplierName = product.Supplier?.Name;
            }

            _logger.LogInformation("Successfully retrieved {ProductCount} products", productDtos.Count());
            return productDtos;
        }

        public async Task<(IEnumerable<ProductDto> Items, int TotalCount)> GetPagedProductsAsync(int pageNumber, int pageSize, string searchTerm = null)
        {
            _logger.LogInformation("Getting paged products - Page: {PageNumber}, Size: {PageSize}, Search: {SearchTerm}",
                pageNumber, pageSize, searchTerm);

            // اعتبارسنجی پارامترهای پیجینیشن
            if (pageNumber < 1)
                throw new ValidationException("Page number must be greater than 0");

            if (pageSize < 1 || pageSize > 100)
                throw new ValidationException("Page size must be between 1 and 100");

            // حالا این خطا نمی‌دهد چون متد به درستی tuple برمی‌گرداند
            var (products, totalCount) = await _productRepository.GetPagedProductsAsync(pageNumber, pageSize, searchTerm);

            var productDtos = _mapper.Map<IEnumerable<ProductDto>>(products);

            foreach (var productDto in productDtos)
            {
                productDto.TotalQuantity = await CalculateTotalQuantityAsync(productDto.Id);
                var product = products.First(p => p.Id == productDto.Id);
                productDto.CategoryName = product.Category?.Name;
                productDto.SupplierName = product.Supplier?.Name;
            }

            _logger.LogInformation("Successfully retrieved {ProductCount} products (page {PageNumber})",
                productDtos.Count(), pageNumber);

            return (productDtos, totalCount);
        }
        public async Task CreateProductAsync(CreateProductDto productDto)
        {
            _logger.LogInformation("Creating new product with SKU: {Sku}", productDto.SKU);

            // اعتبارسنجی SKU تکراری
            var existingProduct = await _productRepository.GetProductBySkuAsync(productDto.SKU);
            if (existingProduct != null)
            {
                _logger.LogWarning("Product with SKU {Sku} already exists", productDto.SKU);
                throw new BusinessException(
                    "Product with this SKU already exists",
                    "SKU_UNIQUE_CONSTRAINT",
                    ErrorCodes.PRODUCT_SKU_DUPLICATE
                );
            }

            // اعتبارسنجی Category
            var category = await _categoryRepository.GetByIdAsync(productDto.CategoryId);
            if (category == null)
            {
                _logger.LogWarning("Category with ID {CategoryId} not found", productDto.CategoryId);
                throw new NotFoundException("Category", productDto.CategoryId, ErrorCodes.NOT_FOUND);
            }

            // اعتبارسنجی Supplier
            var supplier = await _supplierRepository.GetByIdAsync(productDto.SupplierId);
            if (supplier == null)
            {
                _logger.LogWarning("Supplier with ID {SupplierId} not found", productDto.SupplierId);
                throw new NotFoundException("Supplier", productDto.SupplierId, ErrorCodes.NOT_FOUND);
            }

            // اعتبارسنجی سطوح موجودی
            if (productDto.MinimumStockLevel > productDto.MaximumStockLevel)
            {
                throw new BusinessException(
                    "Minimum stock level cannot be greater than maximum stock level",
                    "STOCK_LEVEL_VALIDATION",
                    ErrorCodes.PRODUCT_STOCK_LEVEL_INVALID
                );
            }

            // اعتبارسنجی قیمت
            if (productDto.Price <= 0)
            {
                throw new ValidationException(
                    new Dictionary<string, string[]>
                    {
                        { "Price", new[] { "Price must be greater than 0" } }
                    }
                );
            }

            try
            {
                var product = _mapper.Map<Product>(productDto);
                await _productRepository.AddAsync(product);
                await _productRepository.SaveChangesAsync();

                _logger.LogInformation("Successfully created product with ID: {ProductId} and SKU: {Sku}",
                    product.Id, product.SKU);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error while creating product with SKU: {Sku}", productDto.SKU);
                throw new DataIntegrityException(
                    "An error occurred while saving product to database",
                    "PRODUCT_CREATE_CONSTRAINT",
                    ex
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while creating product with SKU: {Sku}", productDto.SKU);
                throw new BusinessException(
                    "An unexpected error occurred while creating the product",
                    "PRODUCT_CREATE_ERROR",
                    ErrorCodes.UNEXPECTED_ERROR,
                    ex
                );
            }
        }

        public async Task UpdateProductAsync(int id, UpdateProductDto productDto)
        {
            _logger.LogInformation("Updating product with ID: {ProductId}", id);

            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                _logger.LogWarning("Product with ID {ProductId} not found for update", id);
                throw new NotFoundException("Product", id, ErrorCodes.NOT_FOUND);
            }

            // اعتبارسنجی سطوح موجودی
            if (productDto.MinimumStockLevel > productDto.MaximumStockLevel)
            {
                throw new BusinessException(
                    "Minimum stock level cannot be greater than maximum stock level",
                    "STOCK_LEVEL_VALIDATION",
                    ErrorCodes.PRODUCT_STOCK_LEVEL_INVALID
                );
            }

            // اعتبارسنجی قیمت
            if (productDto.Price <= 0)
            {
                throw new ValidationException(
                    new Dictionary<string, string[]>
                    {
                        { "Price", new[] { "Price must be greater than 0" } }
                    }
                );
            }

            try
            {
                // به‌روزرسانی فیلدها
                product.Name = productDto.Name;
                product.Description = productDto.Description;
                product.Price = productDto.Price;
                product.Cost = productDto.Cost;
                product.MinimumStockLevel = productDto.MinimumStockLevel;
                product.MaximumStockLevel = productDto.MaximumStockLevel;
                product.UpdatedAt = DateTime.UtcNow;

                _productRepository.Update(product);
                await _productRepository.SaveChangesAsync();

                _logger.LogInformation("Successfully updated product with ID: {ProductId}", id);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error while updating product with ID: {ProductId}", id);
                throw new DataIntegrityException(
                    "An error occurred while updating product in database",
                    "PRODUCT_UPDATE_CONSTRAINT",
                    ex
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while updating product with ID: {ProductId}", id);
                throw new BusinessException(
                    "An unexpected error occurred while updating the product",
                    "PRODUCT_UPDATE_ERROR",
                    ErrorCodes.UNEXPECTED_ERROR,
                    ex
                );
            }
        }

        public async Task DeleteProductAsync(int id)
        {
            _logger.LogInformation("Deleting product with ID: {ProductId}", id);

            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                _logger.LogWarning("Product with ID {ProductId} not found for deletion", id);
                throw new NotFoundException("Product", id, ErrorCodes.NOT_FOUND);
            }

            try
            {
                // بررسی آیا محصول در موجودی‌ها استفاده شده است
                var hasInventory = await _inventoryRepository.ProductHasInventoryAsync(id);
                if (hasInventory)
                {
                    _logger.LogWarning("Cannot delete product with ID {ProductId} because it has inventory records", id);
                    throw new BusinessException(
                        "Cannot delete product that has inventory records. Please remove inventory first.",
                        "PRODUCT_HAS_INVENTORY",
                        ErrorCodes.PRODUCT_HAS_INVENTORY
                    );
                }

                _productRepository.Delete(product);
                await _productRepository.SaveChangesAsync();

                _logger.LogInformation("Successfully deleted product with ID: {ProductId}", id);
            }
            catch (BusinessException)
            {
                throw;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error while deleting product with ID: {ProductId}", id);
                throw new DataIntegrityException(
                    "An error occurred while deleting product from database",
                    "PRODUCT_DELETE_CONSTRAINT",
                    ex
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while deleting product with ID: {ProductId}", id);
                throw new BusinessException(
                    "An unexpected error occurred while deleting the product",
                    "PRODUCT_DELETE_ERROR",
                    ErrorCodes.UNEXPECTED_ERROR,
                    ex
                );
            }
        }

        public async Task<IEnumerable<ProductDto>> GetLowStockProductsAsync(int threshold = 10)
        {
            _logger.LogInformation("Getting low stock products with threshold: {Threshold}", threshold);

            if (threshold < 0)
            {
                throw new ValidationException("Threshold cannot be negative");
            }

            var products = await _productRepository.GetLowStockProductsAsync(threshold);
            var productDtos = _mapper.Map<IEnumerable<ProductDto>>(products);

            foreach (var productDto in productDtos)
            {
                productDto.TotalQuantity = await CalculateTotalQuantityAsync(productDto.Id);
                var product = products.First(p => p.Id == productDto.Id);
                productDto.CategoryName = product.Category?.Name;
                productDto.SupplierName = product.Supplier?.Name;

                // اضافه کردن هشدار برای محصولات کم‌موجود
                if (productDto.TotalQuantity <= threshold)
                {
                    _logger.LogWarning(
                        "Product {ProductName} (ID: {ProductId}) is low on stock. Current: {CurrentStock}, Threshold: {Threshold}",
                        productDto.Name, productDto.Id, productDto.TotalQuantity, threshold
                    );
                }
            }

            _logger.LogInformation("Found {LowStockCount} low stock products", productDtos.Count());
            return productDtos;
        }

        public async Task<IEnumerable<ProductDto>> SearchProductsAsync(string searchTerm)
        {
            _logger.LogInformation("Searching products with term: {SearchTerm}", searchTerm);

            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return await GetAllProductsAsync();
            }

            if (searchTerm.Length < 2)
            {
                throw new ValidationException("Search term must be at least 2 characters long");
            }

            var products = await _productRepository.SearchProductsAsync(searchTerm);
            var productDtos = _mapper.Map<IEnumerable<ProductDto>>(products);

            foreach (var productDto in productDtos)
            {
                productDto.TotalQuantity = await CalculateTotalQuantityAsync(productDto.Id);
                var product = products.First(p => p.Id == productDto.Id);
                productDto.CategoryName = product.Category?.Name;
                productDto.SupplierName = product.Supplier?.Name;
            }

            _logger.LogInformation("Found {SearchResultCount} products matching search term: {SearchTerm}",
                productDtos.Count(), searchTerm);
            return productDtos;
        }

        public async Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(int categoryId)
        {
            _logger.LogInformation("Getting products by category ID: {CategoryId}", categoryId);

            // اعتبارسنجی وجود دسته‌بندی
            var category = await _categoryRepository.GetByIdAsync(categoryId);
            if (category == null)
            {
                throw new NotFoundException("Category", categoryId, ErrorCodes.NOT_FOUND);
            }

            var products = await _productRepository.GetProductsByCategoryAsync(categoryId);
            var productDtos = _mapper.Map<IEnumerable<ProductDto>>(products);

            foreach (var productDto in productDtos)
            {
                productDto.TotalQuantity = await CalculateTotalQuantityAsync(productDto.Id);
                productDto.CategoryName = category.Name;
                var product = products.First(p => p.Id == productDto.Id);
                productDto.SupplierName = product.Supplier?.Name;
            }

            _logger.LogInformation("Found {CategoryProductCount} products in category ID: {CategoryId}",
                productDtos.Count(), categoryId);
            return productDtos;
        }

        public async Task<bool> IsSkuUniqueAsync(string sku, int? excludeProductId = null)
        {
            if (string.IsNullOrWhiteSpace(sku))
            {
                throw new ValidationException("SKU cannot be empty");
            }

            var product = await _productRepository.GetProductBySkuAsync(sku);
            return product == null || product.Id == excludeProductId;
        }

        public async Task<ProductStatsDto> GetProductStatsAsync(int productId)
        {
            _logger.LogInformation("Getting statistics for product ID: {ProductId}", productId);

            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null)
            {
                throw new NotFoundException("Product", productId, ErrorCodes.NOT_FOUND);
            }

            var totalQuantity = await CalculateTotalQuantityAsync(productId);
            var inventories = await _inventoryRepository.GetByProductIdAsync(productId);

            var stats = new ProductStatsDto
            {
                ProductId = productId,
                ProductName = product.Name,
                TotalQuantity = totalQuantity,
                WarehouseCount = inventories.Count(),
                IsLowStock = totalQuantity <= product.MinimumStockLevel,
                IsOutOfStock = totalQuantity == 0,
                StockValue = totalQuantity * product.Cost,
                LastUpdated = product.UpdatedAt ?? product.CreatedAt
            };

            _logger.LogInformation("Retrieved statistics for product ID: {ProductId}", productId);
            return stats;
        }

        // متد کمکی برای محاسبه موجودی کل
        private async Task<int> CalculateTotalQuantityAsync(int productId)
        {
            try
            {
                var inventories = await _inventoryRepository.GetByProductIdAsync(productId);
                return inventories.Sum(i => i.Quantity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating total quantity for product ID: {ProductId}", productId);
                return 0;
            }
        }
    }
}