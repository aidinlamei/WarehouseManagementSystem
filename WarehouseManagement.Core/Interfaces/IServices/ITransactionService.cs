using MassTransit.Middleware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseManagement.Core.DTOs.FilterDtos;
using WarehouseManagement.Core.DTOs.Transactions;

namespace WarehouseManagement.Core.Interfaces.IServices
{
    public interface ITransactionService
    {
        Task<TransactionDto> GetTransactionByIdAsync(int id);
        Task<IEnumerable<TransactionDto>> GetTransactionsByProductAsync(int productId);
        Task<IEnumerable<TransactionDto>> GetTransactionsByWarehouseAsync(int warehouseId);
        Task<IEnumerable<TransactionDto>> GetTransactionsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<(IEnumerable<TransactionDto> Items, int TotalCount)> GetPagedTransactionsAsync(int pageNumber, int pageSize, TransactionFilter filter = null);
        Task RecordInboundTransactionAsync(InboundTransactionDto transactionDto);
        Task RecordOutboundTransactionAsync(OutboundTransactionDto transactionDto);
        Task RecordAdjustmentTransactionAsync(AdjustmentTransactionDto transactionDto);
        Task<TransactionSummaryDto> GetTransactionSummaryAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<IEnumerable<RecentTransactionDto>> GetRecentTransactionsAsync(int count = 50);
    }
}
