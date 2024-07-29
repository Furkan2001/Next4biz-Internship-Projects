using PasswordManagementSystem.Core.Models;

namespace PasswordManagementSystem.Business.Services
{
    public interface IPasswordRoleService
    {
        Task<IEnumerable<PasswordRole>> GetAllPasswordRoles();
        Task<PasswordRole> GetPasswordRoleById(int id);
        Task CreatePasswordRole(PasswordRole passwordRole);
        Task UpdatePasswordRole(PasswordRole passwordRole);
        Task DeletePasswordRole(int id);
    }
}
