using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseManagement.Core.DTOs.Export;
using WarehouseManagement.Core.DTOs.Inventory;

namespace WarehouseManagement.Core.Interfaces.IServices
{
    public interface IInventoryService
    {
        Task<InventoryDto> GetInventoryAsync(int productId, int warehouseId);
        Task<IEnumerable<InventoryDto>> GetInventoryByProductAsync(int productId);
        Task<IEnumerable<InventoryDto>> GetInventoryByWarehouseAsync(int warehouseId);
        Task<(IEnumerable<InventoryDto> Items, int TotalCount)> GetPagedInventoryAsync(int pageNumber, int pageSize, InventoryFilter filter = null);
        Task AddInventoryAsync(AddInventoryDto inventoryDto);
        Task UpdateInventoryAsync(UpdateInventoryDto inventoryDto);
        Task AdjustInventoryAsync(AdjustInventoryDto adjustDto);
        Task TransferInventoryAsync(TransferInventoryDto transferDto);
        Task ReserveInventoryAsync(ReserveInventoryDto reserveDto);
        Task ReleaseReservedInventoryAsync(ReleaseInventoryDto releaseDto);
        Task<IEnumerable<LowStockItemDto>> GetLowStockItemsAsync(int threshold = 10);
        Task<IEnumerable<OutOfStockItemDto>> GetOutOfStockItemsAsync();
        Task<InventorySummaryDto> GetInventorySummaryAsync();
        Task<int> GetProductTotalQuantityAsync(int productId);
        Task<bool> IsProductAvailableAsync(int productId, int quantity);
    }
}
