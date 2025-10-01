using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Core.Exceptions
{
    public class NotFoundException : AppException
    {
        public string EntityName { get; set; }
        public object EntityId { get; set; }

        public NotFoundException(string message)
            : base(message)
        {
        }

        public NotFoundException(string entityName, object entityId)
            : base($"{entityName} with ID {entityId} not found")
        {
            EntityName = entityName;
            EntityId = entityId;
        }

        public NotFoundException(string entityName, object entityId, string errorCode)
            : base($"{entityName} with ID {entityId} not found", errorCode)
        {
            EntityName = entityName;
            EntityId = entityId;
        }
    }
}
