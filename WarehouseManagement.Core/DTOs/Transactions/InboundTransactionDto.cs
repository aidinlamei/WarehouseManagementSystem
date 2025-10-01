using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Core.DTOs.Transactions
{
    public class InboundTransactionDto
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        public int WarehouseId { get; set; }

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [StringLength(100)]
        public string Reference { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
    }
}
