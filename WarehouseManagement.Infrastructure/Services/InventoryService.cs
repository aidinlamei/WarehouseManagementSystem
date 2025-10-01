using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseManagement.Core.DTOs.Inventory;
using WarehouseManagement.Core.Entities;
using WarehouseManagement.Core.Exceptions;
using WarehouseManagement.Core.Interfaces;
using WarehouseManagement.Core.Interfaces.IServices;

namespace WarehouseManagement.Infrastructure.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly IInventoryRepository _inventoryRepository;
        private readonly IProductRepository _productRepository;
        private readonly IWarehouseRepository _warehouseRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IMapper _mapper;
        private readonly IAppLogger<InventoryService> _logger;

        public InventoryService(
            IInventoryRepository inventoryRepository,
            IProductRepository productRepository,
            IWarehouseRepository warehouseRepository,
            ITransactionRepository transactionRepository,
            IMapper mapper,
            IAppLogger<InventoryService> logger)
        {
            _inventoryRepository = inventoryRepository;
            _productRepository = productRepository;
            _warehouseRepository = warehouseRepository;
            _transactionRepository = transactionRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<InventoryDto> GetInventoryAsync(int productId, int warehouseId)
        {
            _logger.LogInformation("Getting inventory for product {ProductId} in warehouse {WarehouseId}",
                productId, warehouseId);

            var inventory = await _inventoryRepository.GetByProductAndWarehouseAsync(productId, warehouseId);
            if (inventory == null)
            {
                _logger.LogWarning("Inventory not found for product {ProductId} in warehouse {WarehouseId}",
                    productId, warehouseId);
                throw new NotFoundException("Inventory", $"Product:{productId},Warehouse:{warehouseId}", ErrorCodes.INVENTORY_NOT_FOUND);
            }

            var inventoryDto = _mapper.Map<InventoryDto>(inventory);
            inventoryDto.AvailableQuantity = inventory.Quantity - inventory.ReservedQuantity;

            _logger.LogInformation("Successfully retrieved inventory for product {ProductId} in warehouse {WarehouseId}",
                productId, warehouseId);
            return inventoryDto;
        }

        public async Task<IEnumerable<InventoryDto>> GetInventoryByProductAsync(int productId)
        {
            _logger.LogInformation("Getting all inventory for product {ProductId}", productId);

            // اعتبارسنجی محصول
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null)
            {
                throw new NotFoundException("Product", productId, ErrorCodes.NOT_FOUND);
            }

            var inventories = await _inventoryRepository.GetByProductIdAsync(productId);
            var inventoryDtos = _mapper.Map<IEnumerable<InventoryDto>>(inventories);

            foreach (var inventoryDto in inventoryDtos)
            {
                inventoryDto.AvailableQuantity = inventoryDto.Quantity - inventoryDto.ReservedQuantity;
                inventoryDto.ProductName = product.Name;
                inventoryDto.ProductPrice = product.Price;
            }

            _logger.LogInformation("Successfully retrieved {InventoryCount} inventory records for product {ProductId}",
                inventoryDtos.Count(), productId);
            return inventoryDtos;
        }

        public async Task<IEnumerable<InventoryDto>> GetInventoryByWarehouseAsync(int warehouseId)
        {
            _logger.LogInformation("Getting all inventory for warehouse {WarehouseId}", warehouseId);

            // اعتبارسنجی انبار
            var warehouse = await _warehouseRepository.GetByIdAsync(warehouseId);
            if (warehouse == null)
            {
                throw new NotFoundException("Warehouse", warehouseId, ErrorCodes.NOT_FOUND);
            }

            var inventories = await _inventoryRepository.GetByWarehouseIdAsync(warehouseId);
            var inventoryDtos = _mapper.Map<IEnumerable<InventoryDto>>(inventories);

            foreach (var inventoryDto in inventoryDtos)
            {
                inventoryDto.AvailableQuantity = inventoryDto.Quantity - inventoryDto.ReservedQuantity;
                inventoryDto.WarehouseName = warehouse.Name;
            }

            _logger.LogInformation("Successfully retrieved {InventoryCount} inventory records for warehouse {WarehouseId}",
                inventoryDtos.Count(), warehouseId);
            return inventoryDtos;
        }

        public async Task AddInventoryAsync(AddInventoryDto inventoryDto)
        {
            _logger.LogInformation("Adding inventory - Product: {ProductId}, Warehouse: {WarehouseId}, Quantity: {Quantity}",
                inventoryDto.ProductId, inventoryDto.WarehouseId, inventoryDto.Quantity);

            // اعتبارسنجی محصول
            var product = await _productRepository.GetByIdAsync(inventoryDto.ProductId);
            if (product == null)
            {
                throw new NotFoundException("Product", inventoryDto.ProductId, ErrorCodes.NOT_FOUND);
            }

            // اعتبارسنجی انبار
            var warehouse = await _warehouseRepository.GetByIdAsync(inventoryDto.WarehouseId);
            if (warehouse == null)
            {
                throw new NotFoundException("Warehouse", inventoryDto.WarehouseId, ErrorCodes.NOT_FOUND);
            }

            // اعتبارسنجی مقدار
            if (inventoryDto.Quantity <= 0)
            {
                throw new ValidationException("Quantity must be greater than 0");
            }

            try
            {
                // افزودن موجودی
                var inventory = await _inventoryRepository.AddInventoryAsync(
                    inventoryDto.ProductId,
                    inventoryDto.WarehouseId,
                    inventoryDto.Quantity
                );

                // ثبت تراکنش
                var transaction = new Transaction
                {
                    ProductId = inventoryDto.ProductId,
                    WarehouseId = inventoryDto.WarehouseId,
                    Type = TransactionType.Inbound,
                    Quantity = inventoryDto.Quantity,
                    Reference = $"ADD_{DateTime.UtcNow:yyyyMMddHHmmss}",
                    Description = inventoryDto.Reason ?? "Manual inventory addition"
                };

                await _transactionRepository.AddAsync(transaction);
                await _transactionRepository.SaveChangesAsync();

                _logger.LogInformation("Successfully added {Quantity} units of product {ProductId} to warehouse {WarehouseId}",
                    inventoryDto.Quantity, inventoryDto.ProductId, inventoryDto.WarehouseId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding inventory for product {ProductId} in warehouse {WarehouseId}",
                    inventoryDto.ProductId, inventoryDto.WarehouseId);
                throw new BusinessException(
                    "An error occurred while adding inventory",
                    "INVENTORY_ADD_ERROR",
                    ErrorCodes.UNEXPECTED_ERROR,
                    ex
                );
            }
        }

        public async Task UpdateInventoryAsync(UpdateInventoryDto inventoryDto)
        {
            _logger.LogInformation("Updating inventory - Product: {ProductId}, Warehouse: {WarehouseId}, New Quantity: {Quantity}",
                inventoryDto.ProductId, inventoryDto.WarehouseId, inventoryDto.Quantity);

            if (inventoryDto.Quantity < 0)
            {
                throw new ValidationException("Quantity cannot be negative");
            }

            var existingInventory = await _inventoryRepository.GetByProductAndWarehouseAsync(
                inventoryDto.ProductId, inventoryDto.WarehouseId);

            if (existingInventory == null)
            {
                throw new NotFoundException("Inventory",
                    $"Product:{inventoryDto.ProductId},Warehouse:{inventoryDto.WarehouseId}",
                    ErrorCodes.INVENTORY_NOT_FOUND);
            }

            var quantityChange = inventoryDto.Quantity - existingInventory.Quantity;

            try
            {
                // به‌روزرسانی موجودی
                existingInventory.Quantity = inventoryDto.Quantity;
                _inventoryRepository.Update(existingInventory);

                // ثبت تراکنش
                var transactionType = quantityChange > 0 ? TransactionType.Inbound : TransactionType.Adjustment;
                var transaction = new Transaction
                {
                    ProductId = inventoryDto.ProductId,
                    WarehouseId = inventoryDto.WarehouseId,
                    Type = transactionType,
                    Quantity = Math.Abs(quantityChange),
                    Reference = $"ADJ_{DateTime.UtcNow:yyyyMMddHHmmss}",
                    Description = inventoryDto.Reason ?? "Manual inventory adjustment"
                };

                await _transactionRepository.AddAsync(transaction);
                await _inventoryRepository.SaveChangesAsync();
                await _transactionRepository.SaveChangesAsync();

                _logger.LogInformation("Successfully updated inventory for product {ProductId} in warehouse {WarehouseId} to {Quantity}",
                    inventoryDto.ProductId, inventoryDto.WarehouseId, inventoryDto.Quantity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating inventory for product {ProductId} in warehouse {WarehouseId}",
                    inventoryDto.ProductId, inventoryDto.WarehouseId);
                throw new BusinessException(
                    "An error occurred while updating inventory",
                    "INVENTORY_UPDATE_ERROR",
                    ErrorCodes.UNEXPECTED_ERROR,
                    ex
                );
            }
        }

        public async Task AdjustInventoryAsync(AdjustInventoryDto adjustDto)
        {
            _logger.LogInformation("Adjusting inventory - Product: {ProductId}, Warehouse: {WarehouseId}, Adjustment: {Adjustment}",
                adjustDto.ProductId, adjustDto.WarehouseId, adjustDto.Adjustment);

            if (adjustDto.Adjustment == 0)
            {
                throw new ValidationException("Adjustment cannot be zero");
            }

            try
            {
                // اعمال تعدیل
                await _inventoryRepository.UpdateInventoryAsync(
                    adjustDto.ProductId,
                    adjustDto.WarehouseId,
                    adjustDto.Adjustment
                );

                // ثبت تراکنش
                var transactionType = adjustDto.Adjustment > 0 ? TransactionType.Inbound : TransactionType.Outbound;
                var transaction = new Transaction
                {
                    ProductId = adjustDto.ProductId,
                    WarehouseId = adjustDto.WarehouseId,
                    Type = transactionType,
                    Quantity = Math.Abs(adjustDto.Adjustment),
                    Reference = $"ADJ_{DateTime.UtcNow:yyyyMMddHHmmss}",
                    Description = adjustDto.Reason ?? "Inventory adjustment"
                };

                await _transactionRepository.AddAsync(transaction);
                await _transactionRepository.SaveChangesAsync();

                _logger.LogInformation("Successfully adjusted inventory for product {ProductId} in warehouse {WarehouseId} by {Adjustment}",
                    adjustDto.ProductId, adjustDto.WarehouseId, adjustDto.Adjustment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adjusting inventory for product {ProductId} in warehouse {WarehouseId}",
                    adjustDto.ProductId, adjustDto.WarehouseId);
                throw new BusinessException(
                    "An error occurred while adjusting inventory",
                    "INVENTORY_ADJUST_ERROR",
                    ErrorCodes.UNEXPECTED_ERROR,
                    ex
                );
            }
        }

        public async Task TransferInventoryAsync(TransferInventoryDto transferDto)
        {
            _logger.LogInformation("Transferring inventory - Product: {ProductId}, From: {FromWarehouseId}, To: {ToWarehouseId}, Quantity: {Quantity}",
                transferDto.ProductId, transferDto.FromWarehouseId, transferDto.ToWarehouseId, transferDto.Quantity);

            if (transferDto.Quantity <= 0)
            {
                throw new ValidationException("Transfer quantity must be greater than 0");
            }

            if (transferDto.FromWarehouseId == transferDto.ToWarehouseId)
            {
                throw new ValidationException("Source and destination warehouses cannot be the same");
            }

            // بررسی موجودی در انبار مبدا
            var sourceInventory = await _inventoryRepository.GetByProductAndWarehouseAsync(
                transferDto.ProductId, transferDto.FromWarehouseId);

            if (sourceInventory == null || sourceInventory.Quantity < transferDto.Quantity)
            {
                throw new InventoryException(
                    "Insufficient stock for transfer",
                    transferDto.ProductId,
                    transferDto.FromWarehouseId,
                    transferDto.Quantity,
                    sourceInventory?.Quantity ?? 0
                );
            }

            try
            {
                // کسر از انبار مبدا
                sourceInventory.Quantity -= transferDto.Quantity;
                _inventoryRepository.Update(sourceInventory);

                // افزودن به انبار مقصد
                await _inventoryRepository.AddInventoryAsync(
                    transferDto.ProductId,
                    transferDto.ToWarehouseId,
                    transferDto.Quantity
                );

                // ثبت تراکنش خروج از مبدا
                var outboundTransaction = new Transaction
                {
                    ProductId = transferDto.ProductId,
                    WarehouseId = transferDto.FromWarehouseId,
                    Type = TransactionType.Outbound,
                    Quantity = transferDto.Quantity,
                    Reference = $"TRF_OUT_{DateTime.UtcNow:yyyyMMddHHmmss}",
                    Description = $"Transfer to warehouse {transferDto.ToWarehouseId}: {transferDto.Reason}"
                };

                // ثبت تراکنش ورود به مقصد
                var inboundTransaction = new Transaction
                {
                    ProductId = transferDto.ProductId,
                    WarehouseId = transferDto.ToWarehouseId,
                    Type = TransactionType.Inbound,
                    Quantity = transferDto.Quantity,
                    Reference = $"TRF_IN_{DateTime.UtcNow:yyyyMMddHHmmss}",
                    Description = $"Transfer from warehouse {transferDto.FromWarehouseId}: {transferDto.Reason}"
                };

                await _transactionRepository.AddAsync(outboundTransaction);
                await _transactionRepository.AddAsync(inboundTransaction);
                await _inventoryRepository.SaveChangesAsync();
                await _transactionRepository.SaveChangesAsync();

                _logger.LogInformation("Successfully transferred {Quantity} units of product {ProductId} from warehouse {FromWarehouseId} to {ToWarehouseId}",
                    transferDto.Quantity, transferDto.ProductId, transferDto.FromWarehouseId, transferDto.ToWarehouseId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error transferring inventory for product {ProductId} from warehouse {FromWarehouseId} to {ToWarehouseId}",
                    transferDto.ProductId, transferDto.FromWarehouseId, transferDto.ToWarehouseId);
                throw new BusinessException(
                    "An error occurred while transferring inventory",
                    "INVENTORY_TRANSFER_ERROR",
                    ErrorCodes.UNEXPECTED_ERROR,
                    ex
                );
            }
        }

        public async Task ReserveInventoryAsync(ReserveInventoryDto reserveDto)
        {
            _logger.LogInformation("Reserving inventory - Product: {ProductId}, Warehouse: {WarehouseId}, Quantity: {Quantity}",
                reserveDto.ProductId, reserveDto.WarehouseId, reserveDto.Quantity);

            if (reserveDto.Quantity <= 0)
            {
                throw new ValidationException("Reservation quantity must be greater than 0");
            }

            try
            {
                var success = await _inventoryRepository.ReserveInventoryAsync(
                    reserveDto.ProductId,
                    reserveDto.WarehouseId,
                    reserveDto.Quantity
                );

                if (!success)
                {
                    var inventory = await _inventoryRepository.GetByProductAndWarehouseAsync(
                        reserveDto.ProductId, reserveDto.WarehouseId);

                    var availableQuantity = inventory?.Quantity - inventory?.ReservedQuantity ?? 0;

                    throw new InventoryException(
                        "Insufficient stock for reservation",
                        reserveDto.ProductId,
                        reserveDto.WarehouseId,
                        reserveDto.Quantity,
                        availableQuantity
                    );
                }

                _logger.LogInformation("Successfully reserved {Quantity} units of product {ProductId} in warehouse {WarehouseId}",
                    reserveDto.Quantity, reserveDto.ProductId, reserveDto.WarehouseId);
            }
            catch (InventoryException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reserving inventory for product {ProductId} in warehouse {WarehouseId}",
                    reserveDto.ProductId, reserveDto.WarehouseId);
                throw new BusinessException(
                    "An error occurred while reserving inventory",
                    "INVENTORY_RESERVE_ERROR",
                    ErrorCodes.UNEXPECTED_ERROR,
                    ex
                );
            }
        }

        public async Task ReleaseReservedInventoryAsync(ReleaseInventoryDto releaseDto)
        {
            _logger.LogInformation("Releasing reserved inventory - Product: {ProductId}, Warehouse: {WarehouseId}, Quantity: {Quantity}",
                releaseDto.ProductId, releaseDto.WarehouseId, releaseDto.Quantity);

            if (releaseDto.Quantity <= 0)
            {
                throw new ValidationException("Release quantity must be greater than 0");
            }

            try
            {
                var success = await _inventoryRepository.ReleaseReservedInventoryAsync(
                    releaseDto.ProductId,
                    releaseDto.WarehouseId,
                    releaseDto.Quantity
                );

                if (!success)
                {
                    throw new BusinessException(
                        "Cannot release more than reserved quantity",
                        "INSUFFICIENT_RESERVED_QUANTITY",
                        ErrorCodes.BUSINESS_RULE_VIOLATION
                    );
                }

                _logger.LogInformation("Successfully released {Quantity} reserved units of product {ProductId} in warehouse {WarehouseId}",
                    releaseDto.Quantity, releaseDto.ProductId, releaseDto.WarehouseId);
            }
            catch (BusinessException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error releasing reserved inventory for product {ProductId} in warehouse {WarehouseId}",
                    releaseDto.ProductId, releaseDto.WarehouseId);
                throw new BusinessException(
                    "An error occurred while releasing reserved inventory",
                    "INVENTORY_RELEASE_ERROR",
                    ErrorCodes.UNEXPECTED_ERROR,
                    ex
                );
            }
        }

        public async Task<IEnumerable<LowStockItemDto>> GetLowStockItemsAsync(int threshold = 10)
        {
            _logger.LogInformation("Getting low stock items with threshold: {Threshold}", threshold);

            var lowStockItems = await _inventoryRepository.GetLowStockItemsAsync(threshold);
            var lowStockDtos = _mapper.Map<IEnumerable<LowStockItemDto>>(lowStockItems);

            foreach (var dto in lowStockDtos)
            {
                dto.AvailableQuantity = dto.Quantity - dto.ReservedQuantity;
                dto.IsBelowMinimum = dto.Quantity <= dto.ProductMinimumStockLevel;

                if (dto.IsBelowMinimum)
                {
                    _logger.LogWarning(
                        "Product {ProductName} in warehouse {WarehouseName} is below minimum stock level. Current: {Current}, Minimum: {Minimum}",
                        dto.ProductName, dto.WarehouseName, dto.Quantity, dto.ProductMinimumStockLevel
                    );
                }
            }

            _logger.LogInformation("Found {LowStockCount} low stock items", lowStockDtos.Count());
            return lowStockDtos;
        }

        public async Task<IEnumerable<OutOfStockItemDto>> GetOutOfStockItemsAsync()
        {
            _logger.LogInformation("Getting out of stock items");

            var outOfStockItems = await _inventoryRepository.GetOutOfStockItemsAsync();
            var outOfStockDtos = _mapper.Map<IEnumerable<OutOfStockItemDto>>(outOfStockItems);

            _logger.LogInformation("Found {OutOfStockCount} out of stock items", outOfStockDtos.Count());
            return outOfStockDtos;
        }

        public async Task<InventorySummaryDto> GetInventorySummaryAsync()
        {
            _logger.LogInformation("Getting inventory summary");

            var allInventories = await _inventoryRepository.GetAllAsync();
            var products = await _productRepository.GetAllAsync();
            var warehouses = await _warehouseRepository.GetAllAsync();

            var summary = new InventorySummaryDto
            {
                TotalProducts = products.Count(p => !p.IsDeleted),
                TotalWarehouses = warehouses.Count(w => !w.IsDeleted),
                TotalInventoryItems = allInventories.Count(),
                TotalQuantity = allInventories.Sum(i => i.Quantity),
                TotalReservedQuantity = allInventories.Sum(i => i.ReservedQuantity),
                TotalValue = allInventories.Sum(i => i.Quantity * GetProductPrice(i.ProductId, products)),
                LowStockCount = allInventories.Count(i => i.Quantity <= i.Product.MinimumStockLevel),
                OutOfStockCount = allInventories.Count(i => i.Quantity == 0),
                LastUpdated = DateTime.UtcNow
            };

            summary.TotalAvailableQuantity = summary.TotalQuantity - summary.TotalReservedQuantity;

            _logger.LogInformation("Inventory summary: {TotalProducts} products, {TotalQuantity} total quantity, {TotalValue} total value",
                summary.TotalProducts, summary.TotalQuantity, summary.TotalValue);

            return summary;
        }

        public async Task<int> GetProductTotalQuantityAsync(int productId)
        {
            return await _inventoryRepository.GetTotalProductQuantityAsync(productId);
        }

        public async Task<bool> IsProductAvailableAsync(int productId, int quantity)
        {
            var totalQuantity = await GetProductTotalQuantityAsync(productId);
            return totalQuantity >= quantity;
        }

        public async Task<(IEnumerable<InventoryDto> Items, int TotalCount)> GetPagedInventoryAsync(int pageNumber, int pageSize, InventoryFilter filter = null)
        {
            _logger.LogInformation("Getting paged inventory - Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize);

            if (pageNumber < 1)
                throw new ValidationException("Page number must be greater than 0");

            if (pageSize < 1 || pageSize > 100)
                throw new ValidationException("Page size must be between 1 and 100");

            // استفاده از InventoryRepository برای دریافت داده‌های صفحه‌بندی شده
            var (inventories, totalCount) = await _inventoryRepository.GetPagedInventoryAsync(pageNumber, pageSize, filter);
            var inventoryDtos = _mapper.Map<IEnumerable<InventoryDto>>(inventories);

            // محاسبه موجودی قابل دسترس برای هر آیتم
            foreach (var inventoryDto in inventoryDtos)
            {
                inventoryDto.AvailableQuantity = inventoryDto.Quantity - inventoryDto.ReservedQuantity;

                // پر کردن نام محصول و انبار
                var inventory = inventories.First(i => i.Id == inventoryDto.Id);
                inventoryDto.ProductName = inventory.Product?.Name;
                inventoryDto.WarehouseName = inventory.Warehouse?.Name;
                inventoryDto.ProductPrice = inventory.Product?.Price ?? 0;
            }

            _logger.LogInformation("Successfully retrieved {InventoryCount} inventory records (page {PageNumber})",
                inventoryDtos.Count(), pageNumber);

            return (inventoryDtos, totalCount);
        }
        private decimal GetProductPrice(int productId, IEnumerable<Product> products)
        {
            var product = products.FirstOrDefault(p => p.Id == productId);
            return product?.Price ?? 0;
        }
    }
}
