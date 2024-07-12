using Dapper;
using PasswordManagementSystem.Interfaces;
using PasswordManagementSystem.Models;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace PasswordManagementSystem.Repositories
{
    public class PasswordRoleRepository : IPasswordRoleRepository
    {
        private readonly IDbConnection _db;

        public PasswordRoleRepository(IDbConnection db)
        {
            _db = db;
        }

        public async Task<IEnumerable<PasswordRole>> GetAll()
        {
            return await _db.QueryAsync<PasswordRole>("SELECT * FROM PasswordRoles");
        }

        public async Task<PasswordRole> GetById(int id)
        {
            return await _db.QueryFirstOrDefaultAsync<PasswordRole>("SELECT * FROM PasswordRoles WHERE PasswordRoleId = @Id", new { Id = id });
        }

        public async Task Add(PasswordRole entity)
        {
            var sql = "INSERT INTO PasswordRoles (PasswordId, RoleId) VALUES (@PasswordId, @RoleId)";
            await _db.ExecuteAsync(sql, entity);
        }

        public async Task Update(PasswordRole entity)
        {
            var sql = "UPDATE PasswordRoles SET PasswordId = @PasswordId, RoleId = @RoleId WHERE PasswordRoleId = @PasswordRoleId";
            await _db.ExecuteAsync(sql, entity);
        }

        public async Task Delete(int id)
        {
            var sql = "DELETE FROM PasswordRoles WHERE PasswordRoleId = @Id";
            await _db.ExecuteAsync(sql, new { Id = id });
        }
    }
}
