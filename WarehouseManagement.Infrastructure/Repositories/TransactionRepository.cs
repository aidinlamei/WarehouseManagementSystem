using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseManagement.Core.DTOs.FilterDtos;
using WarehouseManagement.Core.Entities;
using WarehouseManagement.Core.Interfaces;
using WarehouseManagement.Infrastructure.Data;

namespace WarehouseManagement.Infrastructure.Repositories
{
    public class TransactionRepository : BaseRepository<Transaction>, ITransactionRepository
    {
        public TransactionRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Transaction>> GetTransactionsByProductAsync(int productId)
        {
            return await _entities
                .Where(t => t.ProductId == productId && !t.IsDeleted)
                .Include(t => t.Product)
                .Include(t => t.Warehouse)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Transaction>> GetTransactionsByWarehouseAsync(int warehouseId)
        {
            return await _entities
                .Where(t => t.WarehouseId == warehouseId && !t.IsDeleted)
                .Include(t => t.Product)
                .ThenInclude(p => p.Category)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Transaction>> GetTransactionsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _entities
                .Where(t => t.CreatedAt >= startDate && t.CreatedAt <= endDate && !t.IsDeleted)
                .Include(t => t.Product)
                .Include(t => t.Warehouse)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Transaction>> GetTransactionsByTypeAsync(TransactionType type)
        {
            return await _entities
                .Where(t => t.Type == type && !t.IsDeleted)
                .Include(t => t.Product)
                .Include(t => t.Warehouse)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<(IEnumerable<Transaction> Items, int TotalCount)> GetPagedTransactionsAsync(int pageNumber, int pageSize, TransactionFilter filter = null)
        {
            var query = _entities.Where(t => !t.IsDeleted);

            if (filter != null)
            {
                if (filter.ProductId.HasValue)
                    query = query.Where(t => t.ProductId == filter.ProductId.Value);

                if (filter.WarehouseId.HasValue)
                    query = query.Where(t => t.WarehouseId == filter.WarehouseId.Value);

                if (filter.Type.HasValue)
                    query = query.Where(t => t.Type == filter.Type.Value);

                if (filter.StartDate.HasValue)
                    query = query.Where(t => t.CreatedAt >= filter.StartDate.Value);

                if (filter.EndDate.HasValue)
                    query = query.Where(t => t.CreatedAt <= filter.EndDate.Value);

                if (!string.IsNullOrEmpty(filter.Reference))
                    query = query.Where(t => t.Reference.Contains(filter.Reference));
            }

            query = query
                .Include(t => t.Product)
                .ThenInclude(p => p.Category)
                .Include(t => t.Product)
                .ThenInclude(p => p.Supplier)
                .Include(t => t.Warehouse);

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(t => t.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task AddTransactionAsync(Transaction transaction)
        {
            await _entities.AddAsync(transaction);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Transaction>> GetRecentTransactionsAsync(int count = 50)
        {
            return await _entities
                .Where(t => !t.IsDeleted)
                .Include(t => t.Product)
                .Include(t => t.Warehouse)
                .OrderByDescending(t => t.CreatedAt)
                .Take(count)
                .ToListAsync();
        }
    }
}