using PasswordManagementSystem.Interfaces;
using PasswordManagementSystem.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PasswordManagementSystem.Services
{
    public class LabelService
    {
        private readonly ILabelRepository _labelRepository;

        public LabelService(ILabelRepository labelRepository)
        {
            _labelRepository = labelRepository;
        }

        public async Task<IEnumerable<Label>> GetAllLabels()
        {
            return await _labelRepository.GetAll();
        }

        public async Task<Label> GetLabelById(int id)
        {
            return await _labelRepository.GetById(id);
        }

        public async Task CreateLabel(Label label)
        {
            await _labelRepository.Add(label);
        }

        public async Task UpdateLabel(Label label)
        {
            await _labelRepository.Update(label);
        }

        public async Task DeleteLabel(int id)
        {
            await _labelRepository.Delete(id);
        }
    }
}
