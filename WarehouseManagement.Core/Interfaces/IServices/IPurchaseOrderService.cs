using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseManagement.Core.DTOs.FilterDtos;
using WarehouseManagement.Core.DTOs.PurchaseOrders;
using WarehouseManagement.Core.Entities;

namespace WarehouseManagement.Core.Interfaces.IServices
{
    public interface IPurchaseOrderService
    {
        Task<PurchaseOrderDto> GetPurchaseOrderByIdAsync(int id);
        Task<IEnumerable<PurchaseOrderDto>> GetAllPurchaseOrdersAsync();
        Task<(IEnumerable<PurchaseOrderDto> Items, int TotalCount)> GetPagedPurchaseOrdersAsync(int pageNumber, int pageSize, PurchaseOrderFilter filter = null);
        Task CreatePurchaseOrderAsync(CreatePurchaseOrderDto purchaseOrderDto);
        Task UpdatePurchaseOrderAsync(int id, UpdatePurchaseOrderDto purchaseOrderDto);
        Task DeletePurchaseOrderAsync(int id);
        Task ApprovePurchaseOrderAsync(int id);
        Task RejectPurchaseOrderAsync(int id, string reason);
        Task CompletePurchaseOrderAsync(int id);
        Task AddItemToPurchaseOrderAsync(int purchaseOrderId, AddPurchaseOrderItemDto itemDto);
        Task RemoveItemFromPurchaseOrderAsync(int purchaseOrderId, int itemId);
        Task UpdatePurchaseOrderItemAsync(int purchaseOrderId, int itemId, UpdatePurchaseOrderItemDto itemDto);
        Task<IEnumerable<PurchaseOrderDto>> GetPurchaseOrdersBySupplierAsync(int supplierId);
        Task<IEnumerable<PurchaseOrderDto>> GetPurchaseOrdersByStatusAsync(OrderStatus status);
        Task<PurchaseOrderSummaryDto> GetPurchaseOrderSummaryAsync();
     }
}
