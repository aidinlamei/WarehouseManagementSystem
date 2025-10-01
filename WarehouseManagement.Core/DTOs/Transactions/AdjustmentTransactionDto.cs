using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Core.DTOs.Transactions
{
    public class AdjustmentTransactionDto
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        public int WarehouseId { get; set; }

        public int Adjustment { get; set; } // می‌تواند مثبت یا منفی باشد

        [StringLength(100)]
        public string Reference { get; set; }

        [Required]
        [StringLength(500)]
        public string Reason { get; set; }

        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
    }
}
