using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Core.Exceptions
{
    public class BusinessException : AppException
    {
        public string BusinessRule { get; set; }

        public BusinessException(string message)
            : base(message)
        {
        }

        public BusinessException(string message, string businessRule)
            : base(message)
        {
            BusinessRule = businessRule;
        }

        public BusinessException(string message, string businessRule, string errorCode)
            : base(message, errorCode)
        {
            BusinessRule = businessRule;
        }

        public BusinessException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public BusinessException(string message, string businessRule, Exception innerException)
            : base(message, innerException)
        {
            BusinessRule = businessRule;
        }

        public BusinessException(string message, string businessRule, string errorCode, Exception innerException)
            : base(message, errorCode, innerException)
        {
            BusinessRule = businessRule;
        }
    }
}
