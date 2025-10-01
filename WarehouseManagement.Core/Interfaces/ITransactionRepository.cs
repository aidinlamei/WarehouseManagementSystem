using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseManagement.Core.DTOs.FilterDtos;
using WarehouseManagement.Core.Entities;

namespace WarehouseManagement.Core.Interfaces
{
    public interface ITransactionRepository : IRepository<Transaction>
    {
        Task<IEnumerable<Transaction>> GetTransactionsByProductAsync(int productId);
        Task<IEnumerable<Transaction>> GetTransactionsByWarehouseAsync(int warehouseId);
        Task<IEnumerable<Transaction>> GetTransactionsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<Transaction>> GetTransactionsByTypeAsync(TransactionType type);
        Task<(IEnumerable<Transaction> Items, int TotalCount)> GetPagedTransactionsAsync(int pageNumber, int pageSize, TransactionFilter filter = null);
        Task AddTransactionAsync(Transaction transaction);
        Task<IEnumerable<Transaction>> GetRecentTransactionsAsync(int count = 50);
    }
}
