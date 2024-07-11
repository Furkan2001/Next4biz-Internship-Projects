using PasswordManagementSystem.Dtos;
using PasswordManagementSystem.Interfaces;
using PasswordManagementSystem.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PasswordManagementSystem.Services
{
    public class RoleService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IUserRepository _userRepository;

        public RoleService(IRoleRepository roleRepository, IUserRepository userRepository)
        {
            _roleRepository = roleRepository;
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<Role>> GetAllRoles()
        {
            return await _roleRepository.GetAll();
        }

        public async Task<Role> GetRoleById(int id)
        {
            return await _roleRepository.GetById(id);
        }

        public async Task CreateRole(Role role)
        {
            await _roleRepository.Add(role);
        }

        public async Task UpdateRole(Role role)
        {
            await _roleRepository.Update(role);
        }

        public async Task DeleteRole(int id)
        {
            await _roleRepository.Delete(id);
        }

        public async Task<List<Role>> GetRolesByCompanyId(int companyId)
        {
            return (await _roleRepository.GetAll()).Where(r => r.CompanyId == companyId).ToList();
        }

        public async Task<bool> SaveRole(RoleDto roleDto)
        {
            var role = new Role
            {
                RoleId = roleDto.RoleId,
                RoleName = roleDto.RoleName,
                CompanyId = roleDto.CompanyId
            };

            if (role.RoleId == 0)
            {
                await _roleRepository.Add(role);
            }
            else
            {
                await _roleRepository.Update(role);
            }

            return true;
        }

        public async Task<bool> DeleteRole2(int roleId)
        {
            await _roleRepository.Delete(roleId);
            return true;
        }
    }
}
