using PasswordManagementSystem.Dtos;
using PasswordManagementSystem.Interfaces;
using PasswordManagementSystem.Models;
using PasswordManagementSystem.Repositories;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PasswordManagementSystem.Services
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(IUserRepository userRepository, IHttpContextAccessor httpContextAccessor)
        {
            _userRepository = userRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IEnumerable<User>> GetAllUsers()
        {
            return await _userRepository.GetAll();
        }

        public async Task<User> GetUserById(int id)
        {
            return await _userRepository.GetById(id);
        }

        public async Task<User> GetUserByEmailAndPassword(string email, string password)
        {
            return await _userRepository.GetByEmailAndPassword(email, password);
        }

        public async Task<IEnumerable<Role>> GetUserRoles(int userId)
        { 
            return await _userRepository.GetUserRoles(userId);
        }
        public async Task<Company> GetUserCompany(int userId)
        {
            return await _userRepository.GetUserCompany(userId);  
        }

        public async Task<IEnumerable<User>> getUsersInCompanyId(int companyId)
        {
            return await _userRepository.GetUserInCompanyId(companyId);
        }

        public async Task<List<User>> GetUsersByRoleId(int roleId)
        {
            return await _userRepository.GetUsersByRoleId(roleId);
        }

        public async Task<User> GetUserAndRolesById(int userId)
        { 
            return await _userRepository.GetUserById(userId);
        }

        public async Task CreateUser(User user)
        {
            await _userRepository.Add(user);
        }

        public async Task UpdateUser(User user)
        {
            await _userRepository.Update(user);
        }

        public async Task DeleteUser(int id)
        {
            await _userRepository.Delete(id);
        }

        public async Task<bool> SaveUser(UserDto userDto)
        {
            Console.WriteLine("saveuser userservice1");

            var user = new User
            {
                UserId = userDto.UserId,
                Name = userDto.Name,
                Email = userDto.Email,
                Password = userDto.Password, // Assuming Password is part of UserDto
                CompanyId = userDto.CompanyId
            };

            Console.WriteLine("saveuser userservice2");

            if (user.UserId == 0)
            {
                Console.WriteLine("insertUser çalışacak");
                return await InsertUser(user, userDto.Roles);
            }
            else
            {
                Console.WriteLine("saveuser userservice 3");
                return await UpdateUser(user, userDto.Roles);
            }
        }

        public async Task<bool> InsertUser(User user, List<int> roleIds)
        {
            return await _userRepository.InsertUser(user, roleIds);
        }

        public async Task<bool> UpdateUser(User user, List<int> roleIds)
        {
            return await _userRepository.UpdateUser(user, roleIds);
        }

        public async Task<bool> DeleteUser2(int userId)
        { 
            return await _userRepository.DeleteUser(userId);
        }

        public async Task<List<Role>> GetAllRoles(HttpContext httpContext)
        {
            var sessionUserId = httpContext.Session.GetInt32("UserId");
            if (!sessionUserId.HasValue)
            {
                throw new Exception("User not logged in");
            }

            var currentUser = await GetUserById(sessionUserId.Value);
            return (await _userRepository.GetAllRoles()).Where(r => r.CompanyId == currentUser.CompanyId).ToList();
        }


        public async Task<List<Company>> GetAllCompanies()
        {
            return (await _userRepository.GetAllCompanies()).ToList();
        }

        public async Task<List<Password>> GetAllPasswords()
        {
            return (await _userRepository.GetAllPasswords()).ToList();
        }
    }
}
