using PasswordManagementSystem.Core.Models;

namespace PasswordManagementSystem.Business.Services
{
    public interface ILabelService
    {
        Task<IEnumerable<Label>> GetUserLabelsAsync(int userId);
        Task<Label> GetLabelAsync(int labelId);
        Task<bool> AddLabelAsync(Label label);
        Task<bool> UpdateLabelAsync(Label label);
        Task<bool> DeleteLabelAsync(int labelId);
        Task<bool> AddOrUpdateLabelAsync(Label label);
        Task<Label> GetLabelByPasswordIdAndUserId(int passwordId, int userId);
        Task<bool> DeleteLabelByPasswordIdAndUserIdAsync(int passwordId, int userId);
    }
}
