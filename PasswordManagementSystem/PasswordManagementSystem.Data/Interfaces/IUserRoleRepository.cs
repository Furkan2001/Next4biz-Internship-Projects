using PasswordManagementSystem.Core.Models;

namespace PasswordManagementSystem.Data.Interfaces
{
    public interface IUserRoleRepository : IRepository<UserRole>
    {
        Task<UserRole> GetByUserId(int userId);
    }
}
