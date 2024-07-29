using PasswordManagementSystem.Core.Models;

namespace PasswordManagementSystem.Business.Services
{
    public interface IUserRoleService
    {
        Task<IEnumerable<UserRole>> GetAllUserRoles();
        Task<UserRole> GetUserRoleById(int id);
        Task CreateUserRole(UserRole userRole);
        Task UpdateUserRole(UserRole userRole);
        Task DeleteUserRole(int id);
        Task<UserRole> GetUserRoleByUserId(int userId);
    }
}
