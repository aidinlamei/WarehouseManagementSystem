using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseManagement.Core.DTOs.Users;

namespace WarehouseManagement.Core.DTOs.Auth
{
    public class AuthResultDto
    {
        public bool Success { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ExpiresAt { get; set; }
        public UserDto User { get; set; }
        public string Message { get; set; }
        public IEnumerable<string> Errors { get; set; } = new List<string>();
    }
}
