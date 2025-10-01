using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Core.DTOs.Reports
{
    public class TransactionReportDto
    {
        public string ReportTitle { get; set; } = "Transaction Report";
        public TransactionReportFilter Filter { get; set; }
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
        public TransactionReportSummaryDto Summary { get; set; }
        public IEnumerable<TransactionDetailDto> Transactions { get; set; } = new List<TransactionDetailDto>();
        public IEnumerable<TransactionTypeSummaryDto> TypeSummaries { get; set; } = new List<TransactionTypeSummaryDto>();
        public IEnumerable<TransactionTrendDto> TransactionTrends { get; set; } = new List<TransactionTrendDto>();
    }
}
