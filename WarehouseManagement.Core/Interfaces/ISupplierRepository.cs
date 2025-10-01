using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseManagement.Core.Entities;

namespace WarehouseManagement.Core.Interfaces
{
    public interface ISupplierRepository : IRepository<Supplier>
    {
        Task<Supplier> GetSupplierWithProductsAsync(int id);
        Task<Supplier> GetSupplierByNameAsync(string name);
        Task<Supplier> GetSupplierByEmailAsync(string email);
        Task<bool> SupplierHasProductsAsync(int supplierId);
        Task<bool> SupplierHasPurchaseOrdersAsync(int supplierId);
        Task<IEnumerable<Supplier>> GetActiveSuppliersAsync();
        Task<IEnumerable<Supplier>> SearchSuppliersAsync(string searchTerm);
        Task<(IEnumerable<Supplier> Items, int TotalCount)> GetPagedSuppliersAsync(int pageNumber, int pageSize, string searchTerm = null);
    }
}
