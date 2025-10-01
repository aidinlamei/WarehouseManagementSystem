using Microsoft.EntityFrameworkCore;
using WarehouseManagement.Core.DTOs.FilterDtos;
using WarehouseManagement.Core.Entities;
using WarehouseManagement.Core.Interfaces;
using WarehouseManagement.Infrastructure.Data;

namespace WarehouseManagement.Infrastructure.Repositories
{
    public class PurchaseOrderRepository : BaseRepository<PurchaseOrder>, IPurchaseOrderRepository
    {
        public PurchaseOrderRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<PurchaseOrder> GetPurchaseOrderWithItemsAsync(int id)
        {
            return await _entities
                .Include(po => po.Supplier)
                .Include(po => po.Items)
                .ThenInclude(poi => poi.Product)
                .FirstOrDefaultAsync(po => po.Id == id && !po.IsDeleted);
        }

        public async Task<PurchaseOrder> GetPurchaseOrderByNumberAsync(string orderNumber)
        {
            return await _entities
                .FirstOrDefaultAsync(po => po.OrderNumber == orderNumber && !po.IsDeleted);
        }

        public async Task<int> GetPurchaseOrderCountBySupplierAsync(int supplierId)
        {
            return await _entities
                .CountAsync(po => po.SupplierId == supplierId && !po.IsDeleted);
        }

        public async Task<IEnumerable<PurchaseOrder>> GetPurchaseOrdersBySupplierAsync(int supplierId)
        {
            return await _entities
                .Where(po => po.SupplierId == supplierId && !po.IsDeleted)
                .Include(po => po.Supplier)
                .Include(po => po.Items)
                .OrderByDescending(po => po.OrderDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<PurchaseOrder>> GetPurchaseOrdersByStatusAsync(OrderStatus status)
        {
            return await _entities
                .Where(po => po.Status == status && !po.IsDeleted)
                .Include(po => po.Supplier)
                .Include(po => po.Items)
                .OrderByDescending(po => po.OrderDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<PurchaseOrder>> GetPendingPurchaseOrdersAsync()
        {
            return await _entities
                .Where(po => (po.Status == OrderStatus.Pending || po.Status == OrderStatus.Approved) && !po.IsDeleted)
                .Include(po => po.Supplier)
                .Include(po => po.Items)
                .OrderBy(po => po.ExpectedDeliveryDate)
                .ToListAsync();
        }

        public async Task<(IEnumerable<PurchaseOrder> Items, int TotalCount)> GetPagedPurchaseOrdersAsync(int pageNumber, int pageSize, PurchaseOrderFilter filter = null)
        {
            var query = _entities.Where(po => !po.IsDeleted);

            if (filter != null)
            {
                if (filter.SupplierId.HasValue)
                    query = query.Where(po => po.SupplierId == filter.SupplierId.Value);

                if (filter.Status.HasValue)
                    query = query.Where(po => po.Status == filter.Status.Value);

                if (filter.StartDate.HasValue)
                    query = query.Where(po => po.OrderDate >= filter.StartDate.Value);

                if (filter.EndDate.HasValue)
                    query = query.Where(po => po.OrderDate <= filter.EndDate.Value);

                if (!string.IsNullOrEmpty(filter.OrderNumber))
                    query = query.Where(po => po.OrderNumber.Contains(filter.OrderNumber));
            }

            query = query
                .Include(po => po.Supplier)
                .Include(po => po.Items)
                .ThenInclude(poi => poi.Product);

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(po => po.OrderDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<bool> PurchaseOrderNumberExistsAsync(string orderNumber)
        {
            return await _entities
                .AnyAsync(po => po.OrderNumber == orderNumber && !po.IsDeleted);
        }

        public async Task UpdatePurchaseOrderStatusAsync(int purchaseOrderId, OrderStatus status)
        {
            var purchaseOrder = await GetByIdAsync(purchaseOrderId);
            if (purchaseOrder != null)
            {
                purchaseOrder.Status = status;
                purchaseOrder.UpdatedAt = DateTime.UtcNow;
                Update(purchaseOrder);
                await SaveChangesAsync();
            }
        }
    }
}