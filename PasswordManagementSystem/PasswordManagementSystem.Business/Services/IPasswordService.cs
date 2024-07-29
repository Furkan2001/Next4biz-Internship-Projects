using PasswordManagementSystem.Core.Models;
using PasswordManagementSystem.Core.ViewModels;

namespace PasswordManagementSystem.Business.Services
{
    public interface IPasswordService
    {
        Task<IEnumerable<Password>> GetAllPasswords();
        Task<Password> GetPasswordById(int id);
        Task CreatePassword(Password password);
        Task UpdatePassword(Password password);
        Task DeletePassword(int id);
        Task<IEnumerable<Password>> GetUserPasswordsAsync(int userId);
        Task<Password> GetPasswordByIdAsync(int passwordId);
        Task<bool> AddPasswordAsync(Password password);
        Task<bool> UpdatePasswordAsync(Password password);
        Task<bool> DeletePasswordAsync(int passwordId);
        Task<IEnumerable<Role>> GetPasswordRolesAsync(int passwordId);
        Task UpdatePasswordRolesAsync(int passwordId, IEnumerable<int> roleIds);
        Task<IEnumerable<Password>> GetCompanyPasswords(int companyId, int page, int pageSize);
        Task<Password> GetPasswordDetailsByIdAsync(int passwordId);
        Task<Password> GetPasswordByIdAsync(int passwordId, int userId);
        Task<IEnumerable<PasswordWithCreatorEmail>> getPasswordsForUser(int viewerUserId);
        Task<int> GetTotalPasswordCount(int companyId);
    }
}
