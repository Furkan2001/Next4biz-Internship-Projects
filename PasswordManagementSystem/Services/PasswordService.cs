using PasswordManagementSystem.Interfaces;
using PasswordManagementSystem.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PasswordManagementSystem.Services
{
    public class PasswordService
    {
        private readonly IPasswordRepository _passwordRepository;

        public PasswordService(IPasswordRepository passwordRepository)
        {
            _passwordRepository = passwordRepository;
        }

        public async Task<IEnumerable<Password>> GetAllPasswords()
        {
            return await _passwordRepository.GetAll();
        }

        public async Task<Password> GetPasswordById(int id)
        {
            return await _passwordRepository.GetById(id);
        }

        public async Task CreatePassword(Password password)
        {
            await _passwordRepository.Add(password);
        }

        public async Task UpdatePassword(Password password)
        {
            await _passwordRepository.Update(password);
        }

        public async Task DeletePassword(int id)
        {
            await _passwordRepository.Delete(id);
        }
    }
}
