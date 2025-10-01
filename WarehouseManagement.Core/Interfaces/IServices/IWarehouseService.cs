using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseManagement.Core.DTOs.Warehouses;

namespace WarehouseManagement.Core.Interfaces.IServices
{
    public interface IWarehouseService
    {
        Task<WarehouseDto> GetWarehouseByIdAsync(int id);
        Task<IEnumerable<WarehouseDto>> GetAllWarehousesAsync();
        Task<(IEnumerable<WarehouseDto> Items, int TotalCount)> GetPagedWarehousesAsync(int pageNumber, int pageSize, string searchTerm = null);
        Task CreateWarehouseAsync(CreateWarehouseDto warehouseDto);
        Task UpdateWarehouseAsync(int id, UpdateWarehouseDto warehouseDto);
        Task DeleteWarehouseAsync(int id);
        Task<WarehouseCapacityDto> GetWarehouseCapacityAsync(int warehouseId);
        Task<IEnumerable<WarehouseWithInventoryDto>> GetWarehousesWithInventoryAsync();
        Task<bool> WarehouseNameExistsAsync(string name, int? excludeWarehouseId = null);
    }
}
