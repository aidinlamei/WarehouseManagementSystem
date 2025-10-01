using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseManagement.Core.DTOs.Users;

namespace WarehouseManagement.Core.Interfaces.IServices
{
    public interface IUserService
    {
        Task<UserDto> GetUserByIdAsync(int id);
        Task<UserDto> GetUserByEmailAsync(string email);
        Task<UserDto> GetUserByUserNameAsync(string userName);
        Task<(IEnumerable<UserDto> Items, int TotalCount)> GetPagedUsersAsync(int pageNumber, int pageSize, string searchTerm = null);
        Task CreateUserAsync(CreateUserDto userDto);
        Task UpdateUserAsync(int id, UpdateUserDto userDto);
        Task DeleteUserAsync(int id);
        Task<bool> UserEmailExistsAsync(string email, int? excludeUserId = null);
        Task<bool> UserNameExistsAsync(string userName, int? excludeUserId = null);
        Task<IEnumerable<string>> GetUserRolesAsync(int userId);
        Task<bool> AssignUserToRoleAsync(int userId, string role);
        Task<bool> RemoveUserFromRoleAsync(int userId, string role);
    }
}
