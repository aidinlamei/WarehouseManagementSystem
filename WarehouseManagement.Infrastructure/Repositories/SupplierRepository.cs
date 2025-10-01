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
    public class SupplierRepository : BaseRepository<Supplier>, ISupplierRepository
    {
        public SupplierRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Supplier> GetSupplierWithProductsAsync(int id)
        {
            return await _entities
                .Include(s => s.Products)
                .ThenInclude(p => p.Category)
                .Include(s => s.PurchaseOrders)
                .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);
        }

        public async Task<Supplier> GetSupplierByNameAsync(string name)
        {
            return await _entities
                .FirstOrDefaultAsync(s => s.Name.ToLower() == name.ToLower() && !s.IsDeleted);
        }

        public async Task<Supplier> GetSupplierByEmailAsync(string email)
        {
            return await _entities
                .FirstOrDefaultAsync(s => s.Email.ToLower() == email.ToLower() && !s.IsDeleted);
        }

        public async Task<bool> SupplierHasProductsAsync(int supplierId)
        {
            return await _entities
                .Include(s => s.Products)
                .AnyAsync(s => s.Id == supplierId && !s.IsDeleted && s.Products.Any(p => !p.IsDeleted));
        }

        public async Task<bool> SupplierHasPurchaseOrdersAsync(int supplierId)
        {
            return await _entities
                .Include(s => s.PurchaseOrders)
                .AnyAsync(s => s.Id == supplierId && !s.IsDeleted && s.PurchaseOrders.Any(po => !po.IsDeleted));
        }

        public async Task<IEnumerable<Supplier>> GetActiveSuppliersAsync()
        {
            return await _entities
                .Where(s => !s.IsDeleted)
                .Include(s => s.Products.Where(p => !p.IsDeleted))
                .Where(s => s.Products.Any(p => !p.IsDeleted))
                .ToListAsync();
        }

        public async Task<IEnumerable<Supplier>> SearchSuppliersAsync(string searchTerm)
        {
            return await _entities
                .Where(s => !s.IsDeleted &&
                           (s.Name.Contains(searchTerm) ||
                            s.Email.Contains(searchTerm) ||
                            s.ContactPerson.Contains(searchTerm) ||
                            s.Phone.Contains(searchTerm)))
                .ToListAsync();
        }

        public async Task<(IEnumerable<Supplier> Items, int TotalCount)> GetPagedSuppliersAsync(int pageNumber, int pageSize, string searchTerm = null)
        {
            var query = _entities.Where(s => !s.IsDeleted);

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(s =>
                    s.Name.Contains(searchTerm) ||
                    s.Email.Contains(searchTerm) ||
                    s.ContactPerson.Contains(searchTerm) ||
                    s.Phone.Contains(searchTerm));
            }

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderBy(s => s.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }
    }
}
