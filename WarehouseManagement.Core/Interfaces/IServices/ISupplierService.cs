using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseManagement.Core.DTOs.Suppliers;
using WarehouseManagement.Core.Entities;

namespace WarehouseManagement.Core.Interfaces.IServices
{
    public interface ISupplierService
    {
        Task<SupplierDto> GetSupplierByIdAsync(int id);
        Task<IEnumerable<SupplierDto>> GetAllSuppliersAsync();
        Task<(IEnumerable<SupplierDto> Items, int TotalCount)> GetPagedSuppliersAsync(int pageNumber, int pageSize, string searchTerm = null);
        Task CreateSupplierAsync(CreateSupplierDto supplierDto);
        Task UpdateSupplierAsync(int id, UpdateSupplierDto supplierDto);
        Task DeleteSupplierAsync(int id);
        Task<IEnumerable<SupplierDto>> GetActiveSuppliersAsync();
        Task<IEnumerable<SupplierWithProductsDto>> GetSuppliersWithProductsAsync();
        Task<bool> SupplierEmailExistsAsync(string email, int? excludeSupplierId = null);
        Task<bool> SupplierNameExistsAsync(string name, int? excludeSupplierId = null);
    }
}
