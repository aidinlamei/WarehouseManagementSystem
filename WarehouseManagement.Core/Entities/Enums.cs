using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Core.Entities
{
    public enum TransactionType
    {
        Inbound,    // ورود به انبار
        Outbound,   // خروج از انبار
        Adjustment, // تعدیل
        Transfer    // انتقال بین انبارها
    }

    public enum OrderStatus
    {
        Draft,      // پیش‌نویس
        Pending,    // در انتظار تایید
        Approved,   // تایید شده
        Rejected,   // رد شده
        Completed,  // تکمیل شده
        Cancelled   // لغو شده
    }
}
