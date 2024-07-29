using log4net;
using PasswordManagementSystem.Data.Interfaces;
using PasswordManagementSystem.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PasswordManagementSystem.Business.Services.Impl
{
    public class LabelService : ILabelService
    {
        private readonly ILabelRepository _labelRepository;

        public LabelService(ILabelRepository labelRepository)
        {
            _labelRepository = labelRepository;
        }

        public async Task<IEnumerable<Label>> GetUserLabelsAsync(int userId)
        {
            return await _labelRepository.GetUserLabelsAsync(userId);
        }

        public async Task<Label> GetLabelAsync(int labelId)
        {
            return await _labelRepository.GetById(labelId);
        }

        public async Task<bool> AddLabelAsync(Label label)
        {
            try
            {
                await _labelRepository.Add(label);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateLabelAsync(Label label)
        {
            try
            {
                await _labelRepository.Update(label);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteLabelAsync(int labelId)
        {
            try
            {
                await _labelRepository.Delete(labelId);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> AddOrUpdateLabelAsync(Label label)
        {
            var existingLabel = await _labelRepository.GetByPasswordIdAndUserId(label.PasswordId, label.UserId);
            if (existingLabel != null)
            {
                existingLabel.LabelText = label.LabelText;
                await _labelRepository.UpdateAsync(existingLabel);
            }
            else
            {
                await _labelRepository.AddAsync(label);
            }
            return true;
        }

        public async Task<Label> GetLabelByPasswordIdAndUserId(int passwordId, int userId)
        {
            return await _labelRepository.GetByPasswordIdAndUserId(passwordId, userId);
        }

        public async Task<bool> DeleteLabelByPasswordIdAndUserIdAsync(int passwordId, int userId)
        {
            return await _labelRepository.DeleteLabelByPasswordIdAndUserIdAsync(passwordId, userId);
        }
    }
}
