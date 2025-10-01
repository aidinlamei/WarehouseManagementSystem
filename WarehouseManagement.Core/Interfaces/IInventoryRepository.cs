using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseManagement.Core.DTOs.Inventory;
using WarehouseManagement.Core.Entities;

namespace WarehouseManagement.Core.Interfaces
{
    public interface IInventoryRepository : IRepository<Inventory>
    {
        Task<Inventory> GetByProductAndWarehouseAsync(int productId, int warehouseId);
        Task<IEnumerable<Inventory>> GetByProductIdAsync(int productId);
        Task<IEnumerable<Inventory>> GetByWarehouseIdAsync(int warehouseId);
        Task<bool> ProductHasInventoryAsync(int productId);
        Task UpdateInventoryAsync(int productId, int warehouseId, int quantityChange);
        Task<int> GetTotalProductQuantityAsync(int productId);
        Task<IEnumerable<Inventory>> GetLowStockItemsAsync(int threshold = 10);
        Task<IEnumerable<Inventory>> GetOutOfStockItemsAsync();
        Task<Inventory> AddInventoryAsync(int productId, int warehouseId, int quantity);
        Task<bool> ReserveInventoryAsync(int productId, int warehouseId, int quantity);
        Task<bool> ReleaseReservedInventoryAsync(int productId, int warehouseId, int quantity);
        Task<(IEnumerable<Inventory> Items, int TotalCount)> GetPagedInventoryAsync(int pageNumber, int pageSize, InventoryFilter filter = null);
    }
}
