using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Core.Exceptions
{
    public class DataIntegrityException : BusinessException
    {
        public string ConstraintName { get; set; }

        public DataIntegrityException(string message)
            : base(message)
        {
        }

        public DataIntegrityException(string message, string constraintName)
            : base(message, "DATA_INTEGRITY_VIOLATION")
        {
            ConstraintName = constraintName;
        }

        public DataIntegrityException(string message, string constraintName, Exception innerException)
            : base(message, "DATA_INTEGRITY_VIOLATION", innerException)
        {
            ConstraintName = constraintName;
        }
    }
}
