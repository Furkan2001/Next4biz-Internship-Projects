using PasswordManagementSystem.Models;

namespace PasswordManagementSystem.Interfaces
{
    public interface IPasswordRoleRepository : IRepository<PasswordRole>
    {
        Task<IEnumerable<Role>> GetRolesByPasswordIdAsync(int passwordId);
        Task UpdatePasswordRolesAsync(int passwordId, IEnumerable<int> roleIds);
        Task DeletePasswordRolesByPasswordIdAsync(int passwordId);
    }
}
