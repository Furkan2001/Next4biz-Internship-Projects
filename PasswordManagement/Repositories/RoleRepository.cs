using Dapper;
using PasswordManagementSystem.Interfaces;
using PasswordManagementSystem.Models;
using System.Collections.Generic;
using System.Data;
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
    }
}
