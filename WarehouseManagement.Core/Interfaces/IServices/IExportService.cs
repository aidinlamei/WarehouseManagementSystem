using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseManagement.Core.DTOs.Export;

namespace WarehouseManagement.Core.Interfaces.IServices
{
    public interface IExportService
    {
        Task<byte[]> ExportProductsToExcelAsync(ProductExportFilter filter = null);
        Task<byte[]> ExportInventoryToExcelAsync(InventoryExportFilter filter = null);
        Task<byte[]> ExportTransactionsToExcelAsync(TransactionExportFilter filter = null);
        Task<byte[]> ExportPurchaseOrdersToExcelAsync(PurchaseOrderExportFilter filter = null);
        Task<byte[]> ExportReportToPdfAsync(string reportType, object reportData);
    }
}
