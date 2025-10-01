using AutoMapper;
using WarehouseManagement.Core.DTOs.Products;
using WarehouseManagement.Core.DTOs.Suppliers;
using WarehouseManagement.Core.Entities;
using WarehouseManagement.Core.Exceptions;
using WarehouseManagement.Core.Interfaces;
using WarehouseManagement.Core.Interfaces.IServices;

namespace WarehouseManagement.Infrastructure.Services
{
    public class SupplierService : ISupplierService
    {
        private readonly ISupplierRepository _supplierRepository;
        private readonly IProductRepository _productRepository;
        private readonly IPurchaseOrderRepository _purchaseOrderRepository;
        private readonly IMapper _mapper;
        private readonly IAppLogger<SupplierService> _logger;

        public SupplierService(
            ISupplierRepository supplierRepository,
            IProductRepository productRepository,
            IPurchaseOrderRepository purchaseOrderRepository,
            IMapper mapper,
            IAppLogger<SupplierService> logger)
        {
            _supplierRepository = supplierRepository;
            _productRepository = productRepository;
            _purchaseOrderRepository = purchaseOrderRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<SupplierDto> GetSupplierByIdAsync(int id)
        {
            _logger.LogInformation("Getting supplier by ID: {SupplierId}", id);

            var supplier = await _supplierRepository.GetSupplierWithProductsAsync(id);
            if (supplier == null)
            {
                _logger.LogWarning("Supplier with ID {SupplierId} not found", id);
                throw new NotFoundException("Supplier", id, ErrorCodes.NOT_FOUND);
            }

            var supplierDto = _mapper.Map<SupplierDto>(supplier);
            supplierDto.ProductCount = supplier.Products?.Count(p => !p.IsDeleted) ?? 0;
            supplierDto.PurchaseOrderCount = supplier.PurchaseOrders?.Count(po => !po.IsDeleted) ?? 0;

            _logger.LogInformation("Successfully retrieved supplier with ID: {SupplierId}", id);
            return supplierDto;
        }

        public async Task<IEnumerable<SupplierDto>> GetAllSuppliersAsync()
        {
            _logger.LogInformation("Getting all suppliers");

            var suppliers = await _supplierRepository.GetAllAsync();
            var supplierDtos = _mapper.Map<IEnumerable<SupplierDto>>(suppliers);

            foreach (var supplierDto in supplierDtos)
            {
                var products = await _productRepository.GetAllAsync();
                supplierDto.ProductCount = products.Count(p => p.SupplierId == supplierDto.Id && !p.IsDeleted);

                var purchaseOrders = await _purchaseOrderRepository.GetAllAsync();
                supplierDto.PurchaseOrderCount = purchaseOrders.Count(po => po.SupplierId == supplierDto.Id && !po.IsDeleted);
            }

            _logger.LogInformation("Successfully retrieved {SupplierCount} suppliers", supplierDtos.Count());
            return supplierDtos;
        }

        public async Task<(IEnumerable<SupplierDto> Items, int TotalCount)> GetPagedSuppliersAsync(int pageNumber, int pageSize, string searchTerm = null)
        {
            _logger.LogInformation("Getting paged suppliers - Page: {PageNumber}, Size: {PageSize}, Search: {SearchTerm}",
                pageNumber, pageSize, searchTerm);

            if (pageNumber < 1)
                throw new ValidationException("Page number must be greater than 0");

            if (pageSize < 1 || pageSize > 100)
                throw new ValidationException("Page size must be between 1 and 100");

            // استفاده از SupplierRepository برای دریافت داده‌های صفحه‌بندی شده
            var (suppliers, totalCount) = await _supplierRepository.GetPagedSuppliersAsync(pageNumber, pageSize, searchTerm);
            var supplierDtos = _mapper.Map<IEnumerable<SupplierDto>>(suppliers);

            foreach (var supplierDto in supplierDtos)
            {
                var products = await _productRepository.GetAllAsync();
                supplierDto.ProductCount = products.Count(p => p.SupplierId == supplierDto.Id && !p.IsDeleted);

                var purchaseOrders = await _purchaseOrderRepository.GetAllAsync();
                supplierDto.PurchaseOrderCount = purchaseOrders.Count(po => po.SupplierId == supplierDto.Id && !po.IsDeleted);
            }

            _logger.LogInformation("Successfully retrieved {SupplierCount} suppliers (page {PageNumber})",
                supplierDtos.Count(), pageNumber);

            return (supplierDtos, totalCount);
        }
        public async Task CreateSupplierAsync(CreateSupplierDto supplierDto)
        {
            _logger.LogInformation("Creating new supplier with name: {SupplierName}", supplierDto.Name);

            // اعتبارسنجی نام تکراری
            var existingSupplier = await _supplierRepository.GetSupplierByNameAsync(supplierDto.Name);
            if (existingSupplier != null)
            {
                _logger.LogWarning("Supplier with name {SupplierName} already exists", supplierDto.Name);
                throw new BusinessException(
                    "Supplier with this name already exists",
                    "SUPPLIER_NAME_UNIQUE",
                    ErrorCodes.BUSINESS_RULE_VIOLATION
                );
            }

            // اعتبارسنجی ایمیل تکراری
            if (!string.IsNullOrEmpty(supplierDto.Email))
            {
                var existingEmail = await _supplierRepository.GetSupplierByEmailAsync(supplierDto.Email);
                if (existingEmail != null)
                {
                    _logger.LogWarning("Supplier with email {SupplierEmail} already exists", supplierDto.Email);
                    throw new BusinessException(
                        "Supplier with this email already exists",
                        "SUPPLIER_EMAIL_UNIQUE",
                        ErrorCodes.BUSINESS_RULE_VIOLATION
                    );
                }
            }

            try
            {
                var supplier = _mapper.Map<Supplier>(supplierDto);
                await _supplierRepository.AddAsync(supplier);
                await _supplierRepository.SaveChangesAsync();

                _logger.LogInformation("Successfully created supplier with ID: {SupplierId} and name: {SupplierName}",
                    supplier.Id, supplier.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating supplier with name: {SupplierName}", supplierDto.Name);
                throw new BusinessException(
                    "An error occurred while creating the supplier",
                    "SUPPLIER_CREATE_ERROR",
                    ErrorCodes.UNEXPECTED_ERROR,
                    ex
                );
            }
        }

        public async Task UpdateSupplierAsync(int id, UpdateSupplierDto supplierDto)
        {
            _logger.LogInformation("Updating supplier with ID: {SupplierId}", id);

            var supplier = await _supplierRepository.GetByIdAsync(id);
            if (supplier == null)
            {
                _logger.LogWarning("Supplier with ID {SupplierId} not found for update", id);
                throw new NotFoundException("Supplier", id, ErrorCodes.NOT_FOUND);
            }

            // اعتبارسنجی نام تکراری
            var existingSupplier = await _supplierRepository.GetSupplierByNameAsync(supplierDto.Name);
            if (existingSupplier != null && existingSupplier.Id != id)
            {
                _logger.LogWarning("Supplier with name {SupplierName} already exists", supplierDto.Name);
                throw new BusinessException(
                    "Supplier with this name already exists",
                    "SUPPLIER_NAME_UNIQUE",
                    ErrorCodes.BUSINESS_RULE_VIOLATION
                );
            }

            // اعتبارسنجی ایمیل تکراری
            if (!string.IsNullOrEmpty(supplierDto.Email))
            {
                var existingEmail = await _supplierRepository.GetSupplierByEmailAsync(supplierDto.Email);
                if (existingEmail != null && existingEmail.Id != id)
                {
                    _logger.LogWarning("Supplier with email {SupplierEmail} already exists", supplierDto.Email);
                    throw new BusinessException(
                        "Supplier with this email already exists",
                        "SUPPLIER_EMAIL_UNIQUE",
                        ErrorCodes.BUSINESS_RULE_VIOLATION
                    );
                }
            }

            try
            {
                supplier.Name = supplierDto.Name;
                supplier.ContactPerson = supplierDto.ContactPerson;
                supplier.Email = supplierDto.Email;
                supplier.Phone = supplierDto.Phone;
                supplier.Address = supplierDto.Address;
                supplier.UpdatedAt = DateTime.UtcNow;

                _supplierRepository.Update(supplier);
                await _supplierRepository.SaveChangesAsync();

                _logger.LogInformation("Successfully updated supplier with ID: {SupplierId}", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating supplier with ID: {SupplierId}", id);
                throw new BusinessException(
                    "An error occurred while updating the supplier",
                    "SUPPLIER_UPDATE_ERROR",
                    ErrorCodes.UNEXPECTED_ERROR,
                    ex
                );
            }
        }

        public async Task DeleteSupplierAsync(int id)
        {
            _logger.LogInformation("Deleting supplier with ID: {SupplierId}", id);

            var supplier = await _supplierRepository.GetByIdAsync(id);
            if (supplier == null)
            {
                _logger.LogWarning("Supplier with ID {SupplierId} not found for deletion", id);
                throw new NotFoundException("Supplier", id, ErrorCodes.NOT_FOUND);
            }

            try
            {
                // بررسی آیا تامین‌کننده دارای محصول است
                var hasProducts = await _supplierRepository.SupplierHasProductsAsync(id);
                if (hasProducts)
                {
                    _logger.LogWarning("Cannot delete supplier with ID {SupplierId} because it has products", id);
                    throw new BusinessException(
                        "Cannot delete supplier that has products. Please remove or reassign products first.",
                        "SUPPLIER_HAS_PRODUCTS",
                        ErrorCodes.SUPPLIER_HAS_PRODUCTS
                    );
                }

                // بررسی آیا تامین‌کننده دارای سفارش خرید است
                var hasPurchaseOrders = await _supplierRepository.SupplierHasPurchaseOrdersAsync(id);
                if (hasPurchaseOrders)
                {
                    _logger.LogWarning("Cannot delete supplier with ID {SupplierId} because it has purchase orders", id);
                    throw new BusinessException(
                        "Cannot delete supplier that has purchase orders. Please remove or reassign purchase orders first.",
                        "SUPPLIER_HAS_PURCHASE_ORDERS",
                        ErrorCodes.BUSINESS_RULE_VIOLATION
                    );
                }

                _supplierRepository.Delete(supplier);
                await _supplierRepository.SaveChangesAsync();

                _logger.LogInformation("Successfully deleted supplier with ID: {SupplierId}", id);
            }
            catch (BusinessException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting supplier with ID: {SupplierId}", id);
                throw new BusinessException(
                    "An error occurred while deleting the supplier",
                    "SUPPLIER_DELETE_ERROR",
                    ErrorCodes.UNEXPECTED_ERROR,
                    ex
                );
            }
        }

        public async Task<IEnumerable<SupplierDto>> GetActiveSuppliersAsync()
        {
            _logger.LogInformation("Getting active suppliers");

            var suppliers = await _supplierRepository.GetActiveSuppliersAsync();
            var supplierDtos = _mapper.Map<IEnumerable<SupplierDto>>(suppliers);

            foreach (var supplierDto in supplierDtos)
            {
                supplierDto.ProductCount = suppliers.First(s => s.Id == supplierDto.Id).Products.Count(p => !p.IsDeleted);

                var purchaseOrders = await _purchaseOrderRepository.GetAllAsync();
                supplierDto.PurchaseOrderCount = purchaseOrders.Count(po => po.SupplierId == supplierDto.Id && !po.IsDeleted);
            }

            _logger.LogInformation("Successfully retrieved {ActiveSupplierCount} active suppliers", supplierDtos.Count());
            return supplierDtos;
        }

        public async Task<IEnumerable<SupplierWithProductsDto>> GetSuppliersWithProductsAsync()
        {
            _logger.LogInformation("Getting suppliers with products");

            var suppliers = await _supplierRepository.GetAllAsync();
            var supplierDtos = _mapper.Map<IEnumerable<SupplierWithProductsDto>>(suppliers);

            foreach (var supplierDto in supplierDtos)
            {
                var products = await _productRepository.GetAllAsync();
                supplierDto.Products = _mapper.Map<IEnumerable<ProductDto>>(
                    products.Where(p => p.SupplierId == supplierDto.Id && !p.IsDeleted)
                );

                // محاسبه موجودی برای هر محصول
                foreach (var product in supplierDto.Products)
                {
                    product.TotalQuantity = await CalculateProductTotalQuantityAsync(product.Id);
                }
            }

            _logger.LogInformation("Successfully retrieved {SupplierCount} suppliers with products", supplierDtos.Count());
            return supplierDtos;
        }

        public async Task<bool> SupplierEmailExistsAsync(string email, int? excludeSupplierId = null)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ValidationException("Supplier email cannot be empty");
            }

            var supplier = await _supplierRepository.GetSupplierByEmailAsync(email);
            return supplier == null || supplier.Id == excludeSupplierId;
        }

        public async Task<bool> SupplierNameExistsAsync(string name, int? excludeSupplierId = null)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ValidationException("Supplier name cannot be empty");
            }

            var supplier = await _supplierRepository.GetSupplierByNameAsync(name);
            return supplier == null || supplier.Id == excludeSupplierId;
        }

        private async Task<int> CalculateProductTotalQuantityAsync(int productId)
        {
            try
            {
                // این متد باید از InventoryRepository استفاده کند
                // برای حالا مقدار ثابت برمی‌گردانیم
                return 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating total quantity for product ID: {ProductId}", productId);
                return 0;
            }
        }
    }
}
