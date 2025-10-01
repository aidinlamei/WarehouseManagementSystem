using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Core.Exceptions
{
    public class AccessDeniedException : AppException
    {
        public string RequiredPermission { get; set; }

        public AccessDeniedException(string message)
            : base(message)
        {
        }

        public AccessDeniedException(string message, string requiredPermission)
            : base(message)
        {
            RequiredPermission = requiredPermission;
        }

        public AccessDeniedException(string message, string requiredPermission, string errorCode)
            : base(message, errorCode)
        {
            RequiredPermission = requiredPermission;
        }
    }
}
