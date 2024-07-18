using PasswordManagementSystem.Models;

namespace PasswordManagementSystem.Interfaces
{
    public interface IRoleRepository : IRepository<Role>
    {
        Task<IEnumerable<Role>> GetRolesByUserIdAsync(int userId);
        Task<IEnumerable<Role>> GetRolesByPasswordIdAsync(int passwordId);
        Task<IEnumerable<Role>> GetRolesByCompanyIdAsync(int companyId);
    }
}
