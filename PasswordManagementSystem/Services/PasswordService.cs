using log4net;
using PasswordManagementSystem.Controllers;
using PasswordManagementSystem.Interfaces;
using PasswordManagementSystem.Models;
using PasswordManagementSystem.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PasswordManagementSystem.Services
{
    public class PasswordService
    {
        private readonly IPasswordRepository _passwordRepository;
        private readonly IPasswordRoleRepository _passwordRoleRepository;
        private readonly ILabelRepository _labelRepository;
        private static readonly ILog log = LogManager.GetLogger(typeof(AdminController));

        public PasswordService(IPasswordRepository passwordRepository, IPasswordRoleRepository passwordRoleRepository, ILabelRepository labelRepository)
        {
            _passwordRepository = passwordRepository;
            _passwordRoleRepository = passwordRoleRepository;
            _labelRepository = labelRepository;
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

        public async Task<IEnumerable<Password>> GetUserPasswordsAsync(int userId)
        {
            return await _passwordRepository.GetAllPasswordsAsync(userId);
        }

        public async Task<Password> GetPasswordByIdAsync(int passwordId)
        {
            return await _passwordRepository.GetPasswordByIdAsync(passwordId);
        }

        public async Task<bool> AddPasswordAsync(Password password)
        {
            try
            {
                await _passwordRepository.AddPasswordAsync(password);
                return true;
            }
            catch (Exception ex)
            {
                log.Error("Error adding password", ex);
                return false;
            }
        }

        public async Task<bool> UpdatePasswordAsync(Password password)
        {
            try
            {
                await _passwordRepository.UpdatePasswordAsync(password);
                return true;
            }
            catch (Exception ex)
            {
                log.Error("Error updating password", ex);
                return false;
            }
        }

        public async Task<bool> DeletePasswordAsync(int passwordId)
        {
            try
            {
                await _labelRepository.DeleteLabelsByPasswordIdAsync(passwordId);
                await _passwordRoleRepository.DeletePasswordRolesByPasswordIdAsync(passwordId);
                await _passwordRepository.DeletePasswordAsync(passwordId);
                return true;
            }
            catch (Exception ex)
            {
                log.Error("Error deleting password", ex);
                return false;
            }
        }

        public async Task<IEnumerable<Role>> GetPasswordRolesAsync(int passwordId)
        {
            return await _passwordRoleRepository.GetRolesByPasswordIdAsync(passwordId);
        }

        public async Task UpdatePasswordRolesAsync(int passwordId, IEnumerable<int> roleIds)
        {
            await _passwordRoleRepository.UpdatePasswordRolesAsync(passwordId, roleIds);
        }

        public async Task<IEnumerable<Password>> GetCompanyPasswords(int companyId)
        {
            return await _passwordRepository.GetCompanyPasswords(companyId);
        }

        public async Task<Password> GetPasswordDetailsByIdAsync(int passwordId)
        {
            return await _passwordRepository.GetPasswordByIdAsync(passwordId);
        }

        public async Task<Password> GetPasswordByIdAsync(int passwordId, int userId)
        {
            return await _passwordRepository.GetPasswordByIdAsync(passwordId, userId);
        }

        public async Task<IEnumerable<PasswordWithCreatorEmail>> getPasswordsForUser(int viewerUserId) 
        {
            return await _passwordRepository.GetViewablePasswordsAsync(viewerUserId);
        }
    }
}
