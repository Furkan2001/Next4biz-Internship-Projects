using PasswordManagementSystem.Core.Dtos;
using PasswordManagementSystem.Core.Models;

namespace PasswordManagementSystem.Data.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User> GetByEmailAndPassword(string email, string password);
        Task<IEnumerable<Role>> GetUserRoles(int userId);
        Task<IEnumerable<User>> GetUserInCompanyId(int companyId);
        Task<Company> GetUserCompany(int userId);
        Task<List<User>> GetUsersByRoleId(int roleId);
        Task<User> GetUserById(int userId);
        Task<bool> InsertUser(User user, List<int> roleIds);
        Task<bool> UpdateUser(User user, List<int> roleIds);
        Task<bool> DeleteUser(int userId);
        Task<IEnumerable<Role>> GetAllRoles();
        Task<IEnumerable<Company>> GetAllCompanies();
        Task<IEnumerable<Password>> GetAllPasswords();
        string HashPassword(string password);
        bool VerifyPassword(string password, string hashedPassword);
        Task<bool> SaveUser(User user, List<int> roleIds);
        Task<bool> CheckUserPassword(int userId, string userPassword);
    }
}
