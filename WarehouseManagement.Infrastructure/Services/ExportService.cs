using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using WarehouseManagement.Core.DTOs;
using WarehouseManagement.Core.DTOs.Export;
using WarehouseManagement.Core.DTOs.Products;
using WarehouseManagement.Core.Exceptions;
using WarehouseManagement.Core.Interfaces;
using WarehouseManagement.Core.Interfaces.IServices;

namespace WarehouseManagement.Infrastructure.Services
{
    public class ExportService : IExportService
    {
        private readonly IProductService _productService;
        private readonly IInventoryService _inventoryService;
        private readonly ITransactionService _transactionService;
        private readonly IPurchaseOrderService _purchaseOrderService;
        private readonly IAppLogger<ExportService> _logger;

        public ExportService(
            IProductService productService,
            IInventoryService inventoryService,
            ITransactionService transactionService,
            IPurchaseOrderService purchaseOrderService,
            IAppLogger<ExportService> logger)
        {
            _productService = productService;
            _inventoryService = inventoryService;
            _transactionService = transactionService;
            _purchaseOrderService = purchaseOrderService;
            _logger = logger;

            // تنظیم LicenseContext برای نسخه ۵
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        public async Task<byte[]> ExportProductsToExcelAsync(ProductExportFilter filter = null)
        {
            try
            {
                _logger.LogInformation("Exporting products to Excel");

                using var package = new ExcelPackage();
                var worksheet = package.Workbook.Worksheets.Add("Products");

                // ایجاد هدرهای جدول
                SetWorksheetHeader(worksheet, new string[] {
                    "ID", "Name", "SKU", "Price", "Cost", "Category",
                    "Supplier", "Quantity", "Min Stock", "Max Stock", "Created Date"
                }, Color.LightBlue);

                // دریافت و فیلتر کردن داده‌ها
                var products = await GetFilteredProductsAsync(filter);

                // پر کردن داده‌ها
                int row = 2;
                foreach (var product in products)
                {
                    worksheet.Cells[row, 1].Value = product.Id;
                    worksheet.Cells[row, 2].Value = product.Name;
                    worksheet.Cells[row, 3].Value = product.SKU;
                    worksheet.Cells[row, 4].Value = product.Price;
                    worksheet.Cells[row, 5].Value = product.Cost;
                    worksheet.Cells[row, 6].Value = product.CategoryName;
                    worksheet.Cells[row, 7].Value = product.SupplierName;
                    worksheet.Cells[row, 8].Value = product.TotalQuantity;
                    worksheet.Cells[row, 9].Value = product.MinimumStockLevel;
                    worksheet.Cells[row, 10].Value = product.MaximumStockLevel;
                    worksheet.Cells[row, 11].Value = product.CreatedAt.ToString("yyyy-MM-dd");

                    row++;
                }

                // اضافه کردن خلاصه
                AddSummarySection(worksheet, row, products);

                // فرمت‌دهی نهایی
                FormatWorksheet(worksheet, row, 11);

                _logger.LogInformation("Successfully exported {ProductCount} products to Excel", products.Count());
                return package.GetAsByteArray();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting products to Excel");
                throw new BusinessException("Error occurred while exporting products to Excel", ex);
            }
        }

        // متدهای کمکی
        private void SetWorksheetHeader(ExcelWorksheet worksheet, string[] headers, Color backgroundColor)
        {
            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cells[1, i + 1].Value = headers[i];
            }

            using (var range = worksheet.Cells[1, 1, 1, headers.Length])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(backgroundColor);
                range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
            }
        }

        private async Task<List<ProductDto>> GetFilteredProductsAsync(ProductExportFilter filter)
        {
            var productsResult = await _productService.GetAllProductsAsync();
            var products = productsResult.ToList();

            if (filter != null)
            {
                if (filter.CategoryId.HasValue)
                    products = products.Where(p => p.CategoryId == filter.CategoryId.Value).ToList();

                if (filter.SupplierId.HasValue)
                    products = products.Where(p => p.SupplierId == filter.SupplierId.Value).ToList();

                if (filter.IncludeLowStock.HasValue && filter.IncludeLowStock.Value)
                    products = products.Where(p => p.TotalQuantity <= p.MinimumStockLevel).ToList();

                if (filter.IncludeOutOfStock.HasValue && filter.IncludeOutOfStock.Value)
                    products = products.Where(p => p.TotalQuantity == 0).ToList();

                if (!string.IsNullOrEmpty(filter.SearchTerm))
                    products = products.Where(p =>
                        p.Name.Contains(filter.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                        p.SKU.Contains(filter.SearchTerm, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            return products;
        }

        private void AddSummarySection(ExcelWorksheet worksheet, int startRow, List<ProductDto> products)
        {
            var summaryRow = startRow + 2;
            worksheet.Cells[summaryRow, 1].Value = "SUMMARY";
            worksheet.Cells[summaryRow, 1].Style.Font.Bold = true;
            worksheet.Cells[summaryRow, 1].Style.Font.Size = 12;

            worksheet.Cells[summaryRow + 1, 1].Value = "Total Products:";
            worksheet.Cells[summaryRow + 1, 2].Value = products.Count;
            worksheet.Cells[summaryRow + 2, 1].Value = "Total Value:";
            worksheet.Cells[summaryRow + 2, 2].Value = products.Sum(p => p.TotalQuantity * p.Price);
            worksheet.Cells[summaryRow + 3, 1].Value = "Low Stock Items:";
            worksheet.Cells[summaryRow + 3, 2].Value = products.Count(p => p.TotalQuantity <= p.MinimumStockLevel);
            worksheet.Cells[summaryRow + 4, 1].Value = "Out of Stock Items:";
            worksheet.Cells[summaryRow + 4, 2].Value = products.Count(p => p.TotalQuantity == 0);
        }

        private void FormatWorksheet(ExcelWorksheet worksheet, int dataRowCount, int columnCount)
        {
            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

            using (var range = worksheet.Cells[1, 1, dataRowCount - 1, columnCount])
            {
                range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
            }
        }

        // سایر متدها (موقتاً NotImplemented)
        public async Task<byte[]> ExportInventoryToExcelAsync(InventoryExportFilter filter = null)
        {
            throw new NotImplementedException();
        }

        public async Task<byte[]> ExportTransactionsToExcelAsync(TransactionExportFilter filter = null)
        {
            throw new NotImplementedException();
        }

        public async Task<byte[]> ExportPurchaseOrdersToExcelAsync(PurchaseOrderExportFilter filter = null)
        {
            throw new NotImplementedException();
        }

        public async Task<byte[]> ExportReportToPdfAsync(string reportType, object reportData)
        {
            throw new NotImplementedException();
        }
    }
}