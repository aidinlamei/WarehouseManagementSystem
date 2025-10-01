using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseManagement.Core.DTOs.FilterDtos;
using WarehouseManagement.Core.Entities;

namespace WarehouseManagement.Core.Interfaces
{
    public interface IPurchaseOrderRepository : IRepository<PurchaseOrder>
    {
        Task<PurchaseOrder> GetPurchaseOrderWithItemsAsync(int id);
        Task<PurchaseOrder> GetPurchaseOrderByNumberAsync(string orderNumber);
        Task<IEnumerable<PurchaseOrder>> GetPurchaseOrdersBySupplierAsync(int supplierId);
        Task<IEnumerable<PurchaseOrder>> GetPurchaseOrdersByStatusAsync(OrderStatus status);
        Task<IEnumerable<PurchaseOrder>> GetPendingPurchaseOrdersAsync();
        Task<(IEnumerable<PurchaseOrder> Items, int TotalCount)> GetPagedPurchaseOrdersAsync(int pageNumber, int pageSize, PurchaseOrderFilter filter = null);
        Task<bool> PurchaseOrderNumberExistsAsync(string orderNumber);
        Task UpdatePurchaseOrderStatusAsync(int purchaseOrderId, OrderStatus status);
        Task<int> GetPurchaseOrderCountBySupplierAsync(int supplierId);
    }
}
