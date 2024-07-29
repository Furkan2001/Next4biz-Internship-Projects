using Dapper;
using PasswordManagementSystem.Data.Interfaces;
using PasswordManagementSystem.Core.Models;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace PasswordManagementSystem.Data.Repositories
{
    public class LabelRepository : ILabelRepository
    {
        private readonly IDbConnection _dbConnection;

        public LabelRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<IEnumerable<Label>> GetAll()
        {
            var query = "SELECT * FROM Label";
            return await _dbConnection.QueryAsync<Label>(query);
        }

        public async Task<Label> GetById(int id)
        {
            var query = "SELECT * FROM Label WHERE LabelId = @LabelId";
            return await _dbConnection.QuerySingleOrDefaultAsync<Label>(query, new { LabelId = id });
        }

        public async Task Add(Label entity)
        {
            var query = "INSERT INTO Label (UserId, LabelText) VALUES (@UserId, @LabelText)";
            await _dbConnection.ExecuteAsync(query, entity);
        }

        public async Task Update(Label entity)
        {
            var query = "UPDATE Label SET LabelText = @LabelText WHERE LabelId = @LabelId";
            await _dbConnection.ExecuteAsync(query, entity);
        }

        public async Task Delete(int id)
        {
            var query = "DELETE FROM Label WHERE LabelId = @LabelId";
            await _dbConnection.ExecuteAsync(query, new { LabelId = id });
        }

        public async Task<IEnumerable<Label>> GetUserLabelsAsync(int userId)
        {
            var query = "SELECT * FROM Label WHERE UserId = @UserId";
            return await _dbConnection.QueryAsync<Label>(query, new { UserId = userId });
        }

        public async Task AddAsync(Label label)
        {
            var query = "INSERT INTO Label (UserId, PasswordId, LabelText) VALUES (@UserId, @PasswordId, @LabelText)";
            await _dbConnection.ExecuteAsync(query, label);
        }

        public async Task UpdateAsync(Label label)
        {
            var query = "UPDATE Label SET LabelText = @LabelText WHERE UserId = @UserId AND PasswordId = @PasswordId";
            await _dbConnection.ExecuteAsync(query, label);
        }

        public async Task<Label> GetByPasswordIdAndUserId(int passwordId, int userId)
        {
            var query = "SELECT * FROM Label WHERE PasswordId = @PasswordId AND UserId = @UserId";
            return await _dbConnection.QuerySingleOrDefaultAsync<Label>(query, new { PasswordId = passwordId, UserId = userId });
        }

        public async Task DeleteLabelsByPasswordIdAsync(int passwordId)
        {
            var sql = "DELETE FROM Label WHERE PasswordId = @PasswordId";
            await _dbConnection.ExecuteAsync(sql, new { PasswordId = passwordId });
        }

        public async Task<bool> DeleteLabelByPasswordIdAndUserIdAsync(int passwordId, int userId)
        {
            // Etiketin var olup olmadığını kontrol et
            var checkSql = "SELECT COUNT(1) FROM Label WHERE PasswordId = @PasswordId AND UserId = @UserId";
            var exists = await _dbConnection.ExecuteScalarAsync<bool>(checkSql, new { PasswordId = passwordId, UserId = userId });

            if (exists)
            {
                // Eğer etiket varsa, sil
                var deleteSql = "DELETE FROM Label WHERE PasswordId = @PasswordId AND UserId = @UserId";
                var result = await _dbConnection.ExecuteAsync(deleteSql, new { PasswordId = passwordId, UserId = userId });
                return result > 0;
            }

            // Eğer etiket yoksa, silme işlemi yapılmaz
            return true;
        }
    }
}
