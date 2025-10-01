using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseManagement.Core.Entities;

namespace WarehouseManagement.Core.Interfaces
{
    public interface IWarehouseRepository : IRepository<Warehouse>
    {
        Task<Warehouse> GetWarehouseWithInventoryAsync(int id);
        Task<Warehouse> GetWarehouseByNameAsync(string name);
        Task<IEnumerable<Warehouse>> GetWarehousesWithCapacityInfoAsync();
        Task<decimal> GetWarehouseUsedCapacityAsync(int warehouseId);
        Task<bool> IsWarehouseFullAsync(int warehouseId);
        Task<(IEnumerable<Warehouse> Items, int TotalCount)> GetPagedWarehousesAsync(int pageNumber, int pageSize, string searchTerm = null);
    }
}
