using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Core.Exceptions
{
    public class ValidationException : AppException
    {
        public Dictionary<string, string[]> Errors { get; set; }

        public ValidationException(string message)
            : base(message)
        {
            Errors = new Dictionary<string, string[]>();
        }

        public ValidationException(string message, string errorCode)
            : base(message, errorCode)
        {
            Errors = new Dictionary<string, string[]>();
        }

        public ValidationException(Dictionary<string, string[]> errors)
            : base("One or more validation errors occurred")
        {
            Errors = errors ?? new Dictionary<string, string[]>();
        }

        public ValidationException(string message, Dictionary<string, string[]> errors)
            : base(message)
        {
            Errors = errors ?? new Dictionary<string, string[]>();
        }
    }
}
