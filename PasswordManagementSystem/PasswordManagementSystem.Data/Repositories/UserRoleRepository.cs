using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using PasswordManagementSystem.Data.Interfaces;
using PasswordManagementSystem.Core.Models;

namespace PasswordManagementSystem.Data.Repositories
{
    public class UserRoleRepository : IUserRoleRepository
    {
        private readonly IDbConnection _db;

        public UserRoleRepository(IDbConnection db)
        {
            _db = db;
        }

        public async Task<IEnumerable<UserRole>> GetAll()
        {
            return await _db.QueryAsync<UserRole>("SELECT * FROM UserRoles");
        }

        public async Task<UserRole> GetById(int id)
        {
            return await _db.QueryFirstOrDefaultAsync<UserRole>("SELECT * FROM UserRoles WHERE UserRoleId = @Id", new { Id = id });
        }

        public async Task Add(UserRole entity)
        {
            var sql = "INSERT INTO UserRoles (UserId, RoleId) VALUES (@UserId, @RoleId)";
            await _db.ExecuteAsync(sql, entity);
        }

        public async Task Update(UserRole entity)
        {
            var sql = "UPDATE UserRoles SET UserId = @UserId, RoleId = @RoleId WHERE UserRoleId = @UserRoleId";
            await _db.ExecuteAsync(sql, entity);
        }

        public async Task Delete(int id)
        {
            var sql = "DELETE FROM UserRoles WHERE UserRoleId = @Id";
            await _db.ExecuteAsync(sql, new { Id = id });
        }

        public async Task<UserRole> GetByUserId(int userId)
        {
            return await _db.QueryFirstOrDefaultAsync<UserRole>("SELECT * FROM UserRoles WHERE UserId = @UserId", new { UserId = userId });
        }
    }
}
