using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseManagement.Core.DTOs.Inventory;
using WarehouseManagement.Core.DTOs.Warehouses;
using WarehouseManagement.Core.Entities;
using WarehouseManagement.Core.Exceptions;
using WarehouseManagement.Core.Interfaces;
using WarehouseManagement.Core.Interfaces.IServices;

namespace WarehouseManagement.Infrastructure.Services
{
    public class WarehouseService : IWarehouseService
    {
        private readonly IWarehouseRepository _warehouseRepository;
        private readonly IInventoryRepository _inventoryRepository;
        private readonly IMapper _mapper;
        private readonly IAppLogger<WarehouseService> _logger;

        public WarehouseService(
            IWarehouseRepository warehouseRepository,
            IInventoryRepository inventoryRepository,
            IMapper mapper,
            IAppLogger<WarehouseService> logger)
        {
            _warehouseRepository = warehouseRepository;
            _inventoryRepository = inventoryRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<WarehouseDto> GetWarehouseByIdAsync(int id)
        {
            _logger.LogInformation("Getting warehouse by ID: {WarehouseId}", id);

            var warehouse = await _warehouseRepository.GetWarehouseWithInventoryAsync(id);
            if (warehouse == null)
            {
                _logger.LogWarning("Warehouse with ID {WarehouseId} not found", id);
                throw new NotFoundException("Warehouse", id, ErrorCodes.NOT_FOUND);
            }

            var warehouseDto = _mapper.Map<WarehouseDto>(warehouse);
            warehouseDto.UsedCapacity = await _warehouseRepository.GetWarehouseUsedCapacityAsync(id);
            warehouseDto.AvailableCapacity = warehouse.Capacity - warehouseDto.UsedCapacity;
            warehouseDto.UtilizationPercentage = warehouse.Capacity > 0 ?
                (warehouseDto.UsedCapacity / warehouse.Capacity) * 100 : 0;
            warehouseDto.ProductCount = warehouse.Inventories?
                .Select(i => i.ProductId)
                .Distinct()
                .Count() ?? 0;
            warehouseDto.TotalItems = warehouse.Inventories?
                .Sum(i => i.Quantity) ?? 0;

            _logger.LogInformation("Successfully retrieved warehouse with ID: {WarehouseId}", id);
            return warehouseDto;
        }

        public async Task<IEnumerable<WarehouseDto>> GetAllWarehousesAsync()
        {
            _logger.LogInformation("Getting all warehouses");

            var warehouses = await _warehouseRepository.GetAllAsync();
            var warehouseDtos = _mapper.Map<IEnumerable<WarehouseDto>>(warehouses);

            foreach (var warehouseDto in warehouseDtos)
            {
                var usedCapacity = await _warehouseRepository.GetWarehouseUsedCapacityAsync(warehouseDto.Id);
                var warehouse = warehouses.First(w => w.Id == warehouseDto.Id);

                warehouseDto.UsedCapacity = usedCapacity;
                warehouseDto.AvailableCapacity = warehouse.Capacity - usedCapacity;
                warehouseDto.UtilizationPercentage = warehouse.Capacity > 0 ?
                    (usedCapacity / warehouse.Capacity) * 100 : 0;

                // محاسبه تعداد محصولات و اقلام
                var inventories = await _inventoryRepository.GetByWarehouseIdAsync(warehouseDto.Id);
                warehouseDto.ProductCount = inventories.Select(i => i.ProductId).Distinct().Count();
                warehouseDto.TotalItems = inventories.Sum(i => i.Quantity);
            }

            _logger.LogInformation("Successfully retrieved {WarehouseCount} warehouses", warehouseDtos.Count());
            return warehouseDtos;
        }

        public async Task<(IEnumerable<WarehouseDto> Items, int TotalCount)> GetPagedWarehousesAsync(int pageNumber, int pageSize, string searchTerm = null)
        {
            _logger.LogInformation("Getting paged warehouses - Page: {PageNumber}, Size: {PageSize}, Search: {SearchTerm}",
                pageNumber, pageSize, searchTerm);

            if (pageNumber < 1)
                throw new ValidationException("Page number must be greater than 0");

            if (pageSize < 1 || pageSize > 100)
                throw new ValidationException("Page size must be between 1 and 100");

            // استفاده از WarehouseRepository برای دریافت داده‌های صفحه‌بندی شده
            var (warehouses, totalCount) = await _warehouseRepository.GetPagedWarehousesAsync(pageNumber, pageSize, searchTerm);
            var warehouseDtos = _mapper.Map<IEnumerable<WarehouseDto>>(warehouses);

            // محاسبه اطلاعات ظرفیت و موجودی برای هر انبار
            foreach (var warehouseDto in warehouseDtos)
            {
                var usedCapacity = await _warehouseRepository.GetWarehouseUsedCapacityAsync(warehouseDto.Id);
                var warehouse = warehouses.First(w => w.Id == warehouseDto.Id);

                warehouseDto.UsedCapacity = usedCapacity;
                warehouseDto.AvailableCapacity = warehouse.Capacity - usedCapacity;
                warehouseDto.UtilizationPercentage = warehouse.Capacity > 0 ?
                    (usedCapacity / warehouse.Capacity) * 100 : 0;

                // محاسبه تعداد محصولات و اقلام
                var inventories = await _inventoryRepository.GetByWarehouseIdAsync(warehouseDto.Id);
                warehouseDto.ProductCount = inventories.Select(i => i.ProductId).Distinct().Count();
                warehouseDto.TotalItems = inventories.Sum(i => i.Quantity);
            }

            _logger.LogInformation("Successfully retrieved {WarehouseCount} warehouses (page {PageNumber})",
                warehouseDtos.Count(), pageNumber);

            return (warehouseDtos, totalCount);
        }
        public async Task CreateWarehouseAsync(CreateWarehouseDto warehouseDto)
        {
            _logger.LogInformation("Creating new warehouse with name: {WarehouseName}", warehouseDto.Name);

            // اعتبارسنجی نام تکراری
            var existingWarehouse = await _warehouseRepository.GetWarehouseByNameAsync(warehouseDto.Name);
            if (existingWarehouse != null)
            {
                _logger.LogWarning("Warehouse with name {WarehouseName} already exists", warehouseDto.Name);
                throw new BusinessException(
                    "Warehouse with this name already exists",
                    "WAREHOUSE_NAME_UNIQUE",
                    ErrorCodes.BUSINESS_RULE_VIOLATION
                );
            }

            // اعتبارسنجی ظرفیت
            if (warehouseDto.Capacity <= 0)
            {
                throw new ValidationException("Warehouse capacity must be greater than 0");
            }

            try
            {
                var warehouse = _mapper.Map<Warehouse>(warehouseDto);
                await _warehouseRepository.AddAsync(warehouse);
                await _warehouseRepository.SaveChangesAsync();

                _logger.LogInformation("Successfully created warehouse with ID: {WarehouseId} and name: {WarehouseName}",
                    warehouse.Id, warehouse.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating warehouse with name: {WarehouseName}", warehouseDto.Name);
                throw new BusinessException(
                    "An error occurred while creating the warehouse",
                    "WAREHOUSE_CREATE_ERROR",
                    ErrorCodes.UNEXPECTED_ERROR,
                    ex
                );
            }
        }

        public async Task UpdateWarehouseAsync(int id, UpdateWarehouseDto warehouseDto)
        {
            _logger.LogInformation("Updating warehouse with ID: {WarehouseId}", id);

            var warehouse = await _warehouseRepository.GetByIdAsync(id);
            if (warehouse == null)
            {
                _logger.LogWarning("Warehouse with ID {WarehouseId} not found for update", id);
                throw new NotFoundException("Warehouse", id, ErrorCodes.NOT_FOUND);
            }

            // اعتبارسنجی نام تکراری
            var existingWarehouse = await _warehouseRepository.GetWarehouseByNameAsync(warehouseDto.Name);
            if (existingWarehouse != null && existingWarehouse.Id != id)
            {
                _logger.LogWarning("Warehouse with name {WarehouseName} already exists", warehouseDto.Name);
                throw new BusinessException(
                    "Warehouse with this name already exists",
                    "WAREHOUSE_NAME_UNIQUE",
                    ErrorCodes.BUSINESS_RULE_VIOLATION
                );
            }

            // اعتبارسنجی ظرفیت
            if (warehouseDto.Capacity <= 0)
            {
                throw new ValidationException("Warehouse capacity must be greater than 0");
            }

            // بررسی اینکه ظرفیت جدید کمتر از فضای استفاده شده نباشد
            var usedCapacity = await _warehouseRepository.GetWarehouseUsedCapacityAsync(id);
            if (warehouseDto.Capacity < usedCapacity)
            {
                throw new BusinessException(
                    $"New capacity ({warehouseDto.Capacity}) cannot be less than currently used capacity ({usedCapacity})",
                    "CAPACITY_VALIDATION",
                    ErrorCodes.BUSINESS_RULE_VIOLATION
                );
            }

            try
            {
                warehouse.Name = warehouseDto.Name;
                warehouse.Location = warehouseDto.Location;
                warehouse.Capacity = warehouseDto.Capacity;
                warehouse.UpdatedAt = DateTime.UtcNow;

                _warehouseRepository.Update(warehouse);
                await _warehouseRepository.SaveChangesAsync();

                _logger.LogInformation("Successfully updated warehouse with ID: {WarehouseId}", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating warehouse with ID: {WarehouseId}", id);
                throw new BusinessException(
                    "An error occurred while updating the warehouse",
                    "WAREHOUSE_UPDATE_ERROR",
                    ErrorCodes.UNEXPECTED_ERROR,
                    ex
                );
            }
        }

        public async Task DeleteWarehouseAsync(int id)
        {
            _logger.LogInformation("Deleting warehouse with ID: {WarehouseId}", id);

            var warehouse = await _warehouseRepository.GetByIdAsync(id);
            if (warehouse == null)
            {
                _logger.LogWarning("Warehouse with ID {WarehouseId} not found for deletion", id);
                throw new NotFoundException("Warehouse", id, ErrorCodes.NOT_FOUND);
            }

            try
            {
                // بررسی آیا انبار دارای موجودی است
                var inventories = await _inventoryRepository.GetByWarehouseIdAsync(id);
                if (inventories.Any())
                {
                    _logger.LogWarning("Cannot delete warehouse with ID {WarehouseId} because it has inventory", id);
                    throw new BusinessException(
                        "Cannot delete warehouse that has inventory. Please remove or transfer inventory first.",
                        "WAREHOUSE_HAS_INVENTORY",
                        ErrorCodes.BUSINESS_RULE_VIOLATION
                    );
                }

                _warehouseRepository.Delete(warehouse);
                await _warehouseRepository.SaveChangesAsync();

                _logger.LogInformation("Successfully deleted warehouse with ID: {WarehouseId}", id);
            }
            catch (BusinessException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting warehouse with ID: {WarehouseId}", id);
                throw new BusinessException(
                    "An error occurred while deleting the warehouse",
                    "WAREHOUSE_DELETE_ERROR",
                    ErrorCodes.UNEXPECTED_ERROR,
                    ex
                );
            }
        }

        public async Task<WarehouseCapacityDto> GetWarehouseCapacityAsync(int warehouseId)
        {
            _logger.LogInformation("Getting capacity for warehouse ID: {WarehouseId}", warehouseId);

            var warehouse = await _warehouseRepository.GetByIdAsync(warehouseId);
            if (warehouse == null)
            {
                throw new NotFoundException("Warehouse", warehouseId, ErrorCodes.NOT_FOUND);
            }

            var usedCapacity = await _warehouseRepository.GetWarehouseUsedCapacityAsync(warehouseId);
            var availableCapacity = warehouse.Capacity - usedCapacity;
            var utilizationPercentage = warehouse.Capacity > 0 ?
                (usedCapacity / warehouse.Capacity) * 100 : 0;

            var capacityDto = new WarehouseCapacityDto
            {
                WarehouseId = warehouseId,
                WarehouseName = warehouse.Name,
                TotalCapacity = warehouse.Capacity,
                UsedCapacity = usedCapacity,
                AvailableCapacity = availableCapacity,
                UtilizationPercentage = utilizationPercentage,
                IsFull = await _warehouseRepository.IsWarehouseFullAsync(warehouseId),
                LastUpdated = DateTime.UtcNow
            };

            _logger.LogInformation("Retrieved capacity for warehouse ID: {WarehouseId}", warehouseId);
            return capacityDto;
        }

        public async Task<IEnumerable<WarehouseWithInventoryDto>> GetWarehousesWithInventoryAsync()
        {
            _logger.LogInformation("Getting warehouses with inventory");

            var warehouses = await _warehouseRepository.GetWarehousesWithCapacityInfoAsync();
            var warehouseDtos = _mapper.Map<IEnumerable<WarehouseWithInventoryDto>>(warehouses);

            foreach (var warehouseDto in warehouseDtos)
            {
                var usedCapacity = await _warehouseRepository.GetWarehouseUsedCapacityAsync(warehouseDto.Id);
                var warehouse = warehouses.First(w => w.Id == warehouseDto.Id);

                warehouseDto.UsedCapacity = usedCapacity;
                warehouseDto.AvailableCapacity = warehouse.Capacity - usedCapacity;
                warehouseDto.UtilizationPercentage = warehouse.Capacity > 0 ?
                    (usedCapacity / warehouse.Capacity) * 100 : 0;

                // محاسبه موجودی‌ها
                var inventories = await _inventoryRepository.GetByWarehouseIdAsync(warehouseDto.Id);
                warehouseDto.InventoryItems = _mapper.Map<IEnumerable<InventoryDto>>(inventories);

                warehouseDto.TotalProducts = warehouseDto.InventoryItems.Select(i => i.ProductId).Distinct().Count();
                warehouseDto.TotalQuantity = warehouseDto.InventoryItems.Sum(i => i.Quantity);
                warehouseDto.TotalValue = warehouseDto.InventoryItems.Sum(i => i.Quantity * i.ProductPrice);
            }

            _logger.LogInformation("Successfully retrieved {WarehouseCount} warehouses with inventory", warehouseDtos.Count());
            return warehouseDtos;
        }

        public async Task<bool> WarehouseNameExistsAsync(string name, int? excludeWarehouseId = null)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ValidationException("Warehouse name cannot be empty");
            }

            var warehouse = await _warehouseRepository.GetWarehouseByNameAsync(name);
            return warehouse == null || warehouse.Id == excludeWarehouseId;
        }
    }
}
