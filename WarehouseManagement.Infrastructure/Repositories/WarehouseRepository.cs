using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseManagement.Core.Entities;
using WarehouseManagement.Core.Interfaces;
using WarehouseManagement.Infrastructure.Data;

namespace WarehouseManagement.Infrastructure.Repositories
{
    public class WarehouseRepository : BaseRepository<Warehouse>, IWarehouseRepository
    {
        public WarehouseRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Warehouse> GetWarehouseWithInventoryAsync(int id)
        {
            return await _entities
                .Include(w => w.Inventories)
                .ThenInclude(i => i.Product)
                .ThenInclude(p => p.Category)
                .Include(w => w.Inventories)
                .ThenInclude(i => i.Product)
                .ThenInclude(p => p.Supplier)
                .FirstOrDefaultAsync(w => w.Id == id && !w.IsDeleted);
        }

        public async Task<Warehouse> GetWarehouseByNameAsync(string name)
        {
            return await _entities
                .FirstOrDefaultAsync(w => w.Name.ToLower() == name.ToLower() && !w.IsDeleted);
        }

        public async Task<IEnumerable<Warehouse>> GetWarehousesWithCapacityInfoAsync()
        {
            return await _entities
                .Where(w => !w.IsDeleted)
                .Include(w => w.Inventories)
                .ToListAsync();
        }

        public async Task<decimal> GetWarehouseUsedCapacityAsync(int warehouseId)
        {
            var warehouse = await _entities
                .Include(w => w.Inventories)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(w => w.Id == warehouseId && !w.IsDeleted);

            if (warehouse == null)
                return 0;

            // فرض: هر محصول 0.1 متر مربع فضا اشغال می‌کند
            // این منطق را می‌توانید بر اساس نیاز تغییر دهید
            decimal totalSpaceUsed = warehouse.Inventories
                .Where(i => !i.Product.IsDeleted)
                .Sum(i => i.Quantity * 0.1m);

            return totalSpaceUsed;
        }

        public async Task<bool> IsWarehouseFullAsync(int warehouseId)
        {
            var warehouse = await GetByIdAsync(warehouseId);
            if (warehouse == null)
                return true;

            var usedCapacity = await GetWarehouseUsedCapacityAsync(warehouseId);
            return usedCapacity >= warehouse.Capacity;
        }

        public async Task<(IEnumerable<Warehouse> Items, int TotalCount)> GetPagedWarehousesAsync(int pageNumber, int pageSize, string searchTerm = null)
        {
            var query = _entities.Where(w => !w.IsDeleted);

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(w =>
                    w.Name.Contains(searchTerm) ||
                    w.Location.Contains(searchTerm));
            }

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderBy(w => w.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }
    }
}
