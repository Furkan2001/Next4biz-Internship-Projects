using PasswordManagementSystem.Core.Dtos;
using PasswordManagementSystem.Core.Models;

namespace PasswordManagementSystem.Business.Services
{
    public interface IRoleService
    {
        Task<IEnumerable<Role>> GetAllRoles();
        Task<Role> GetRoleById(int id);
        Task CreateRole(Role role);
        Task UpdateRole(Role role);
        Task DeleteRole(int id);
        Task<List<Role>> GetRolesByCompanyId(int companyId);
        Task<bool> SaveRole(RoleDto roleDto);
        Task<bool> DeleteRole2(int roleId);
        Task<IEnumerable<Role>> GetAllRolesAsync();
        Task<IEnumerable<Role>> GetRolesByCompanyIdAsync(int companyId);
        Task<IEnumerable<Role>> GetRolesByUserIdAsync(int userId);
        Task<IEnumerable<Role>> GetRolesByPasswordIdAsync(int passwordId);
    }
}
