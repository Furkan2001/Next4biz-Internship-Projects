using PasswordManagementSystem.Data.Interfaces;
using PasswordManagementSystem.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PasswordManagementSystem.Business.Services.Impl
{
    public class PasswordRoleService : IPasswordRoleService
    {
        private readonly IPasswordRoleRepository _passwordRoleRepository;

        public PasswordRoleService(IPasswordRoleRepository passwordRoleRepository)
        {
            _passwordRoleRepository = passwordRoleRepository;
        }

        public async Task<IEnumerable<PasswordRole>> GetAllPasswordRoles()
        {
            return await _passwordRoleRepository.GetAll();
        }

        public async Task<PasswordRole> GetPasswordRoleById(int id)
        {
            return await _passwordRoleRepository.GetById(id);
        }

        public async Task CreatePasswordRole(PasswordRole passwordRole)
        {
            await _passwordRoleRepository.Add(passwordRole);
        }

        public async Task UpdatePasswordRole(PasswordRole passwordRole)
        {
            await _passwordRoleRepository.Update(passwordRole);
        }

        public async Task DeletePasswordRole(int id)
        {
            await _passwordRoleRepository.Delete(id);
        }
    }
}
