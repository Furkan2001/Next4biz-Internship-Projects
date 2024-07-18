using Dapper;
using PasswordManagementSystem.Interfaces;
using PasswordManagementSystem.Models;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace PasswordManagementSystem.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly IDbConnection _db;

        public RoleRepository(IDbConnection db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Role>> GetAll()
        {
            return await _db.QueryAsync<Role>("SELECT * FROM Role");
        }

        public async Task<Role> GetById(int id)
        {
            return await _db.QueryFirstOrDefaultAsync<Role>("SELECT * FROM Role WHERE RoleId = @Id", new { Id = id });
        }

        public async Task Add(Role entity)
        {
            var sql = "INSERT INTO Role (RoleName, CompanyId) VALUES (@RoleName, @CompanyId)";
            await _db.ExecuteAsync(sql, entity);
        }

        public async Task Update(Role entity)
        {
            var sql = "UPDATE Role SET RoleName = @RoleName WHERE RoleId = @RoleId";
            await _db.ExecuteAsync(sql, entity);
        }

        public async Task Delete(int id)
        {
            await _db.ExecuteAsync("DELETE FROM Role WHERE RoleId = @Id", new { Id = id });
        }

        public async Task<IEnumerable<Role>> GetRolesByCompanyIdAsync(int companyId)
        {
            var query = "SELECT * FROM Role WHERE CompanyId = @CompanyId";
            return await _db.QueryAsync<Role>(query, new { CompanyId = companyId });
        }

        public async Task<IEnumerable<Role>> GetRolesByUserIdAsync(int userId)
        {
            var query = @"
            SELECT r.* FROM Role r
            JOIN UserRole ur ON r.RoleId = ur.RoleId
            WHERE ur.UserId = @UserId";
            return await _db.QueryAsync<Role>(query, new { UserId = userId });
        }

        public async Task<IEnumerable<Role>> GetRolesByPasswordIdAsync(int passwordId)
        {
            var query = @"
            SELECT r.* FROM Role r
            JOIN PasswordRole pr ON r.RoleId = pr.RoleId
            WHERE pr.PasswordId = @PasswordId";
            return await _db.QueryAsync<Role>(query, new { PasswordId = passwordId });
        }
    }
}
