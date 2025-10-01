using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseManagement.Core.DTOs.Inventory;
using WarehouseManagement.Core.Entities;
using WarehouseManagement.Core.Interfaces;
using WarehouseManagement.Infrastructure.Data;

namespace WarehouseManagement.Infrastructure.Repositories
{
    public class InventoryRepository : BaseRepository<Inventory>, IInventoryRepository
    {
        public InventoryRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Inventory> GetByProductAndWarehouseAsync(int productId, int warehouseId)
        {
            return await _entities
                .Include(i => i.Product)
                .Include(i => i.Warehouse)
                .FirstOrDefaultAsync(i => i.ProductId == productId && i.WarehouseId == warehouseId && !i.IsDeleted);
        }

        public async Task<IEnumerable<Inventory>> GetByProductIdAsync(int productId)
        {
            return await _entities
                .Where(i => i.ProductId == productId && !i.IsDeleted)
                .Include(i => i.Warehouse)
                .Include(i => i.Product)
                .ThenInclude(p => p.Category)
                .ToListAsync();
        }

        public async Task<IEnumerable<Inventory>> GetByWarehouseIdAsync(int warehouseId)
        {
            return await _entities
                .Where(i => i.WarehouseId == warehouseId && !i.IsDeleted)
                .Include(i => i.Product)
                .ThenInclude(p => p.Category)
                .Include(i => i.Product)
                .ThenInclude(p => p.Supplier)
                .ToListAsync();
        }

        public async Task<bool> ProductHasInventoryAsync(int productId)
        {
            return await _entities
                .AnyAsync(i => i.ProductId == productId && !i.IsDeleted && i.Quantity > 0);
        }

        public async Task UpdateInventoryAsync(int productId, int warehouseId, int quantityChange)
        {
            var inventory = await GetByProductAndWarehouseAsync(productId, warehouseId);

            if (inventory == null)
            {
                // اگر موجودی وجود ندارد، ایجاد می‌کنیم
                inventory = new Inventory
                {
                    ProductId = productId,
                    WarehouseId = warehouseId,
                    Quantity = quantityChange > 0 ? quantityChange : 0,
                    ReservedQuantity = 0
                };
                await AddAsync(inventory);
            }
            else
            {
                // به‌روزرسانی موجودی موجود
                inventory.Quantity += quantityChange;
                if (inventory.Quantity < 0)
                    inventory.Quantity = 0;

                Update(inventory);
            }

            await SaveChangesAsync();
        }

        public async Task<int> GetTotalProductQuantityAsync(int productId)
        {
            return await _entities
                .Where(i => i.ProductId == productId && !i.IsDeleted)
                .SumAsync(i => i.Quantity);
        }

        public async Task<IEnumerable<Inventory>> GetLowStockItemsAsync(int threshold = 10)
        {
            return await _entities
                .Where(i => !i.IsDeleted && i.Quantity <= threshold)
                .Include(i => i.Product)
                .ThenInclude(p => p.Category)
                .Include(i => i.Product)
                .ThenInclude(p => p.Supplier)
                .Include(i => i.Warehouse)
                .ToListAsync();
        }

        public async Task<IEnumerable<Inventory>> GetOutOfStockItemsAsync()
        {
            return await _entities
                .Where(i => !i.IsDeleted && i.Quantity == 0)
                .Include(i => i.Product)
                .ThenInclude(p => p.Category)
                .Include(i => i.Product)
                .ThenInclude(p => p.Supplier)
                .Include(i => i.Warehouse)
                .ToListAsync();
        }

        public async Task<Inventory> AddInventoryAsync(int productId, int warehouseId, int quantity)
        {
            var inventory = await GetByProductAndWarehouseAsync(productId, warehouseId);

            if (inventory == null)
            {
                inventory = new Inventory
                {
                    ProductId = productId,
                    WarehouseId = warehouseId,
                    Quantity = quantity,
                    ReservedQuantity = 0
                };
                await AddAsync(inventory);
            }
            else
            {
                inventory.Quantity += quantity;
                Update(inventory);
            }

            await SaveChangesAsync();
            return inventory;
        }

        public async Task<bool> ReserveInventoryAsync(int productId, int warehouseId, int quantity)
        {
            var inventory = await GetByProductAndWarehouseAsync(productId, warehouseId);

            if (inventory == null || inventory.Quantity - inventory.ReservedQuantity < quantity)
            {
                return false; // موجودی کافی نیست
            }

            inventory.ReservedQuantity += quantity;
            Update(inventory);
            await SaveChangesAsync();

            return true;
        }

        public async Task<bool> ReleaseReservedInventoryAsync(int productId, int warehouseId, int quantity)
        {
            var inventory = await GetByProductAndWarehouseAsync(productId, warehouseId);

            if (inventory == null || inventory.ReservedQuantity < quantity)
            {
                return false; // مقدار رزرو شده کافی نیست
            }

            inventory.ReservedQuantity -= quantity;
            Update(inventory);
            await SaveChangesAsync();

            return true;
        }

        public async Task<(IEnumerable<Inventory> Items, int TotalCount)> GetPagedInventoryAsync(int pageNumber, int pageSize, InventoryFilter filter = null)
        {
            var query = _entities.Where(i => !i.IsDeleted);

            if (filter != null)
            {
                if (filter.ProductId.HasValue)
                    query = query.Where(i => i.ProductId == filter.ProductId.Value);

                if (filter.WarehouseId.HasValue)
                    query = query.Where(i => i.WarehouseId == filter.WarehouseId.Value);

                if (filter.CategoryId.HasValue)
                    query = query.Where(i => i.Product.CategoryId == filter.CategoryId.Value);

                if (filter.LowStockOnly)
                    query = query.Where(i => i.Quantity <= i.Product.MinimumStockLevel);

                if (filter.OutOfStockOnly)
                    query = query.Where(i => i.Quantity == 0);
            }

            query = query
                .Include(i => i.Product)
                .ThenInclude(p => p.Category)
                .Include(i => i.Product)
                .ThenInclude(p => p.Supplier)
                .Include(i => i.Warehouse);

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderBy(i => i.Product.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }
    }
}
