using Dapper;
using PasswordManagementSystem.Interfaces;
using PasswordManagementSystem.Models;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace PasswordManagementSystem.Repositories
{
    public class LabelRepository : ILabelRepository
    {
        private readonly IDbConnection _db;

        public LabelRepository(IDbConnection db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Label>> GetAll()
        {
            return await _db.QueryAsync<Label>("SELECT * FROM Labels");
        }

        public async Task<Label> GetById(int id)
        {
            return await _db.QueryFirstOrDefaultAsync<Label>("SELECT * FROM Labels WHERE LabelId = @Id", new { Id = id });
        }

        public async Task Add(Label entity)
        {
            var sql = "INSERT INTO Labels (UserId, PasswordId, LabelText) VALUES (@UserId, @PasswordId, @LabelText)";
            await _db.ExecuteAsync(sql, entity);
        }

        public async Task Update(Label entity)
        {
            var sql = "UPDATE Labels SET UserId = @UserId, PasswordId = @PasswordId, LabelText = @LabelText WHERE LabelId = @LabelId";
            await _db.ExecuteAsync(sql, entity);
        }

        public async Task Delete(int id)
        {
            var sql = "DELETE FROM Labels WHERE LabelId = @Id";
            await _db.ExecuteAsync(sql, new { Id = id });
        }
    }
}
