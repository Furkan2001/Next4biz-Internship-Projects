using Dapper;
using PasswordManagementSystem.Interfaces;
using PasswordManagementSystem.Models;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace PasswordManagementSystem.Repositories
{
    public class PasswordRepository : IPasswordRepository
    {
        private readonly IDbConnection _db;

        public PasswordRepository(IDbConnection db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Password>> GetAll()
        {
            return await _db.QueryAsync<Password>("SELECT * FROM Passwords");
        }

        public async Task<Password> GetById(int id)
        {
            return await _db.QueryFirstOrDefaultAsync<Password>("SELECT * FROM Passwords WHERE PasswordId = @Id", new { Id = id });
        }

        public async Task Add(Password entity)
        {
            var sql = "INSERT INTO Passwords (UserId, EncryptedPassword, PasswordName) VALUES (@UserId, @EncryptedPassword, @PasswordName)";
            await _db.ExecuteAsync(sql, entity);
        }

        public async Task Update(Password entity)
        {
            var sql = "UPDATE Passwords SET UserId = @UserId, EncryptedPassword = @EncryptedPassword, PasswordName = @PasswordName WHERE PasswordId = @PasswordId";
            await _db.ExecuteAsync(sql, entity);
        }

        public async Task Delete(int id)
        {
            var sql = "DELETE FROM Passwords WHERE PasswordId = @Id";
            await _db.ExecuteAsync(sql, new { Id = id });
        }
    }
}
