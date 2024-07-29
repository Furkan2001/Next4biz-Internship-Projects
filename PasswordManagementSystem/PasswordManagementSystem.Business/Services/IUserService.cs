using PasswordManagementSystem.Core.Dtos;
using PasswordManagementSystem.Core.Models;
using Microsoft.AspNetCore.Http;

namespace PasswordManagementSystem.Business.Services
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllUsers();
        Task<User> GetUserById(int id);
        Task<User> GetUserByEmailAndPassword(string email, string password);
        Task<IEnumerable<Role>> GetUserRoles(int userId);
        Task<Company> GetUserCompany(int userId);
        Task<IEnumerable<User>> getUsersInCompanyId(int companyId);
        Task<List<User>> GetUsersByRoleId(int roleId);
        Task<User> GetUserAndRolesById(int userId);
        Task CreateUser(User user);
        Task UpdateUser(User user);
        Task DeleteUser(int id);
        Task<bool> InsertUser(User user, List<int> roleIds);
        Task<bool> UpdateUser(User user, List<int> roleIds);
        Task<bool> DeleteUser2(int userId);
        Task<List<Role>> GetAllRoles(HttpContext httpContext);
        Task<List<Company>> GetAllCompanies();
        Task<List<Password>> GetAllPasswords();
        Task<bool> SaveUser(UserDto userDto);
        Task<bool> CheckUserPassword(int userId, string userPassword);
    }
}
