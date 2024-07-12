using PasswordManagementSystem.Models;

namespace PasswordManagementSystem.Interfaces
{
    public interface IUserRoleRepository : IRepository<UserRole>
    {
        Task<UserRole> GetByUserId(int userId);
    }
}
