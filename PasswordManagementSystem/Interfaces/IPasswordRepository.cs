using PasswordManagementSystem.Models;
using PasswordManagementSystem.ViewModels;

namespace PasswordManagementSystem.Interfaces
{
    public interface IPasswordRepository : IRepository<Password>
    {
        Task<IEnumerable<Password>> GetAllPasswordsAsync(int userId);
        Task<Password> GetPasswordByIdAsync(int passwordId);
        Task AddPasswordAsync(Password password);
        Task UpdatePasswordAsync(Password password);
        Task DeletePasswordAsync(int passwordId);
        Task<IEnumerable<Password>> GetCompanyPasswords(int companyId);
        Task<Password> GetPasswordByIdAsync(int passwordId, int userId);
        Task<IEnumerable<PasswordWithCreatorEmail>> GetViewablePasswordsAsync(int viewerUserId);
    }
}
