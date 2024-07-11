using System.Collections.Generic;
using System.Threading.Tasks;
using PasswordManagementSystem.Interfaces;
using PasswordManagementSystem.Models;

namespace PasswordManagementSystem.Services
{
    public class UserRoleService
    {
        private readonly IUserRoleRepository _userRoleRepository;

        public UserRoleService(IUserRoleRepository userRoleRepository)
        {
            _userRoleRepository = userRoleRepository;
        }

        public async Task<IEnumerable<UserRole>> GetAllUserRoles()
        {
            return await _userRoleRepository.GetAll();
        }

        public async Task<UserRole> GetUserRoleById(int id)
        {
            return await _userRoleRepository.GetById(id);
        }

        public async Task CreateUserRole(UserRole userRole)
        {
            await _userRoleRepository.Add(userRole);
        }

        public async Task UpdateUserRole(UserRole userRole)
        {
            await _userRoleRepository.Update(userRole);
        }

        public async Task DeleteUserRole(int id)
        {
            await _userRoleRepository.Delete(id);
        }

        public async Task<UserRole> GetUserRoleByUserId(int userId)
        {
            return await _userRoleRepository.GetByUserId(userId);
        }
    }
}
