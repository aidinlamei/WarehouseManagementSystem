using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Core.Exceptions
{
    public abstract class AppException : Exception
    {
        public string ErrorCode { get; set; }
        public DateTime Timestamp { get; set; }

        protected AppException(string message) : base(message)
        {
            Timestamp = DateTime.UtcNow;
        }

        protected AppException(string message, string errorCode) : base(message)
        {
            ErrorCode = errorCode;
            Timestamp = DateTime.UtcNow;
        }

        protected AppException(string message, Exception innerException) : base(message, innerException)
        {
            Timestamp = DateTime.UtcNow;
        }

        protected AppException(string message, string errorCode, Exception innerException)
            : base(message, innerException)
        {
            ErrorCode = errorCode;
            Timestamp = DateTime.UtcNow;
        }
    }
}
