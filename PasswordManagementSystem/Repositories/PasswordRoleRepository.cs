using Dapper;
using PasswordManagementSystem.Interfaces;
using PasswordManagementSystem.Models;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
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

        public async Task<IEnumerable<Role>> GetRolesByPasswordIdAsync(int passwordId)
        {
            var query = @"SELECT r.* FROM Role r
                      INNER JOIN PasswordRole pr ON r.RoleId = pr.RoleId
                      WHERE pr.PasswordId = @PasswordId";
            return await _db.QueryAsync<Role>(query, new { PasswordId = passwordId });
        }

        // Daha önceki roleid leri sil ve güncel olarak eklenmiş olan roleid leri ekle
        public async Task UpdatePasswordRolesAsync(int passwordId, IEnumerable<int> roleIds)
        {
            var deleteQuery = "DELETE FROM PasswordRole WHERE PasswordId = @PasswordId";
            await _db.ExecuteAsync(deleteQuery, new { PasswordId = passwordId });

            var insertQuery = "INSERT INTO PasswordRole (PasswordId, RoleId) VALUES (@PasswordId, @RoleId)";

            if (roleIds != null)
            {
                foreach (var roleId in roleIds)
                {
                    await _db.ExecuteAsync(insertQuery, new { PasswordId = passwordId, RoleId = roleId });
                }
            }
        }

        public async Task DeletePasswordRolesByPasswordIdAsync(int passwordId)
        {
            var sql = "DELETE FROM PasswordRole WHERE PasswordId = @PasswordId";
            await _db.ExecuteAsync(sql, new { PasswordId = passwordId });
        }
    }
}
