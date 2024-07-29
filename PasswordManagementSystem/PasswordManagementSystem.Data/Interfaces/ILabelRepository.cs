using PasswordManagementSystem.Core.Models;

namespace PasswordManagementSystem.Data.Interfaces
{
    public interface ILabelRepository : IRepository<Label>
    {
        Task AddAsync(Label label);
        Task UpdateAsync(Label label);
        Task<Label> GetByPasswordIdAndUserId(int passwordId, int userId);
        Task<IEnumerable<Label>> GetUserLabelsAsync(int userId);
        Task DeleteLabelsByPasswordIdAsync(int passwordId);
        Task<bool> DeleteLabelByPasswordIdAndUserIdAsync(int passwordId, int userId);
    }
}
