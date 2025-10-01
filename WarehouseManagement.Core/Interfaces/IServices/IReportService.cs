using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseManagement.Core.DTOs.FilterDtos;
using WarehouseManagement.Core.DTOs.Reports;

namespace WarehouseManagement.Core.Interfaces.IServices
{
    public interface IReportService
    {
        Task<InventoryReportDto> GenerateInventoryReportAsync(InventoryReportFilter filter = null);
        Task<SalesReportDto> GenerateSalesReportAsync(DateTime startDate, DateTime endDate);
        Task<StockMovementReportDto> GenerateStockMovementReportAsync(DateTime startDate, DateTime endDate);
        Task<SupplierPerformanceReportDto> GenerateSupplierPerformanceReportAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<WarehouseUtilizationReportDto> GenerateWarehouseUtilizationReportAsync();
        Task<ProductPerformanceReportDto> GenerateProductPerformanceReportAsync(DateTime startDate, DateTime endDate);
        Task<LowStockReportDto> GenerateLowStockReportAsync(int threshold = 10);
        Task<TransactionReportDto> GenerateTransactionReportAsync(TransactionReportFilter filter);
    }
}
