using PasswordManagementSystem.Core.Models;

namespace PasswordManagementSystem.Data.Interfaces
{
    public interface IRoleRepository : IRepository<Role>
    {
        Task<IEnumerable<Role>> GetRolesByUserIdAsync(int userId);
        Task<IEnumerable<Role>> GetRolesByPasswordIdAsync(int passwordId);
        Task<IEnumerable<Role>> GetRolesByCompanyIdAsync(int companyId);
    }
}
