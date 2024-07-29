using Dapper;
using PasswordManagementSystem.Core.Helpers;
using PasswordManagementSystem.Data.Interfaces;
using PasswordManagementSystem.Core.Models;
using PasswordManagementSystem.Core.ViewModels;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace PasswordManagementSystem.Data.Repositories
{
    public class PasswordRepository : IPasswordRepository
    {
        private readonly IDbConnection _dbConnection;
        private readonly DataProtectionHelper _dataProtectionHelper;

        public PasswordRepository(IDbConnection dbConnection, DataProtectionHelper dataProtectionHelper)
        {
            _dbConnection = dbConnection;
            _dataProtectionHelper = dataProtectionHelper;
        }

        public async Task<IEnumerable<Password>> GetAll()
        {
            var query = "SELECT * FROM Password";
            var result = await _dbConnection.QueryAsync<Password>(query);
            foreach (var password in result)
            {
                password.EncryptedPassword = _dataProtectionHelper.Decrypt(password.EncryptedPassword);
            }
            return result;
        }

        public async Task<Password> GetById(int id)
        {
            var query = "SELECT * FROM Password WHERE PasswordId = @PasswordId";
            var result = await _dbConnection.QueryAsync<Password>(query, new { PasswordId = id });
            var password = result.FirstOrDefault();
            if (password != null)
            {
                password.EncryptedPassword = _dataProtectionHelper.Decrypt(password.EncryptedPassword);
            }
            return password;
        }

        public async Task Add(Password entity)
        {
            entity.EncryptedPassword = _dataProtectionHelper.Encrypt(entity.EncryptedPassword);
            var query = "INSERT INTO Password (UserId, PasswordName, EncryptedPassword, LabelId) VALUES (@UserId, @PasswordName, @EncryptedPassword, @LabelId)";
            await _dbConnection.ExecuteAsync(query, entity);
        }

        public async Task Update(Password entity)
        {
            entity.EncryptedPassword = _dataProtectionHelper.Encrypt(entity.EncryptedPassword);
            var query = "UPDATE Password SET PasswordName = @PasswordName, EncryptedPassword = @EncryptedPassword, LabelId = @LabelId WHERE PasswordId = @PasswordId";
            await _dbConnection.ExecuteAsync(query, entity);
        }

        public async Task Delete(int id)
        {
            var query = "DELETE FROM Password WHERE PasswordId = @PasswordId";
            await _dbConnection.ExecuteAsync(query, new { PasswordId = id });
        }

        public async Task<IEnumerable<Password>> GetAllPasswordsAsync(int userId)
        {
            var query = "SELECT * FROM Password WHERE UserId = @UserId";
            var result = await _dbConnection.QueryAsync<Password>(query, new { UserId = userId });
            foreach (var password in result)
            {
                password.EncryptedPassword = _dataProtectionHelper.Decrypt(password.EncryptedPassword);
            }
            return result;
        }

        public async Task<Password> GetPasswordByIdAsync(int passwordId)
        {
            var query = "SELECT * FROM Password WHERE PasswordId = @PasswordId";
            var result = await _dbConnection.QueryAsync<Password>(query, new { PasswordId = passwordId });
            var password = result.FirstOrDefault();
            if (password != null)
            {
                password.EncryptedPassword = _dataProtectionHelper.Decrypt(password.EncryptedPassword);
            }
            return password;
        }

        public async Task AddPasswordAsync(Password password)
        {
            password.EncryptedPassword = _dataProtectionHelper.Encrypt(password.EncryptedPassword);
            var query = "INSERT INTO Password (UserId, PasswordName, EncryptedPassword) VALUES (@UserId, @PasswordName, @EncryptedPassword); SELECT CAST(SCOPE_IDENTITY() as int)";
            var id = await _dbConnection.QuerySingleAsync<int>(query, password);
            password.PasswordId = id; // Ensure the ID is set
        }

        public async Task UpdatePasswordAsync(Password password)
        {
            password.EncryptedPassword = _dataProtectionHelper.Encrypt(password.EncryptedPassword);
            var query = "UPDATE Password SET PasswordName = @PasswordName, EncryptedPassword = @EncryptedPassword WHERE PasswordId = @PasswordId";
            await _dbConnection.ExecuteAsync(query, password);
        }

        public async Task DeletePasswordAsync(int passwordId)
        {
            var query = "DELETE FROM Password WHERE PasswordId = @PasswordId";
            await _dbConnection.ExecuteAsync(query, new { PasswordId = passwordId });
        }

        public async Task<IEnumerable<Password>> GetCompanyPasswords(int companyId, int page, int pageSize)
        {
            var query = $@"
            SELECT p.*, u.Email AS CreatorEmail 
            FROM Password p 
            INNER JOIN [User] u ON p.UserId = u.UserId 
            WHERE u.CompanyId = @CompanyId
            ORDER BY p.PasswordId
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            var result = await _dbConnection.QueryAsync<Password>(query, new
            {
                CompanyId = companyId,
                Offset = (page - 1) * pageSize,
                PageSize = pageSize
            });

            foreach (var password in result)
            {
                password.EncryptedPassword = _dataProtectionHelper.Decrypt(password.EncryptedPassword);
                password.Labels = await GetLabelsForPassword(password.PasswordId);
            }

            return result;
        }

        public async Task<IEnumerable<PasswordWithCreatorEmail>> GetViewablePasswordsAsync(int viewerUserId)
        {
            var query = @"
                    SELECT DISTINCT p.PasswordId, p.PasswordName, p.EncryptedPassword, p.UserId, u.Email AS CreatedBy,
                        CASE 
                            WHEN p.UserId = @ViewerUserId THEN 1
                            WHEN (SELECT MIN(RoleId) FROM UserRole WHERE UserId = p.UserId) > (SELECT MIN(RoleId) FROM UserRole WHERE UserId = @ViewerUserId) THEN 1
                            WHEN EXISTS (SELECT 1 FROM PasswordRole pr INNER JOIN UserRole ur ON pr.RoleId = ur.RoleId WHERE pr.PasswordId = p.PasswordId AND ur.UserId = @ViewerUserId) THEN 0
                            ELSE 0
                        END AS CanEdit
                    FROM Password p
                    INNER JOIN [User] u ON p.UserId = u.UserId
                    LEFT JOIN PasswordRole pr ON p.PasswordId = pr.PasswordId
                    LEFT JOIN UserRole ur ON u.UserId = ur.UserId
                    LEFT JOIN UserRole ur_viewer ON ur_viewer.UserId = @ViewerUserId
                    WHERE u.CompanyId = (SELECT CompanyId FROM [User] WHERE UserId = @ViewerUserId)
                      AND (p.UserId = @ViewerUserId
                           OR ur_viewer.RoleId IN (SELECT RoleId FROM PasswordRole WHERE PasswordId = p.PasswordId)
                           OR (SELECT MIN(ur.RoleId) FROM UserRole ur WHERE ur.UserId = p.UserId) > (SELECT MIN(ur_viewer.RoleId) FROM UserRole ur_viewer WHERE ur_viewer.UserId = @ViewerUserId))";

            var result = await _dbConnection.QueryAsync<PasswordWithCreatorEmail>(query, new { ViewerUserId = viewerUserId });

            foreach (var password in result)
            {
                password.EncryptedPassword = _dataProtectionHelper.Decrypt(password.EncryptedPassword);
                password.Labels = await GetLabelsForPassword(password.PasswordId);
            }

            return result;
        }


        private async Task<List<Label>> GetLabelsForPassword(int passwordId)
        {
            var labelQuery = "SELECT LabelId, PasswordId, UserId, LabelText FROM Label WHERE PasswordId = @PasswordId";
            var labels = await _dbConnection.QueryAsync<Label>(labelQuery, new { PasswordId = passwordId });
            return labels.ToList();
        }

        public async Task<Password> GetPasswordDetailsByIdAsync(int passwordId)
        {
            var query = @"SELECT p.*, u.Email AS CreatorEmail 
                          FROM Password p 
                          INNER JOIN [User] u ON p.UserId = u.UserId 
                          WHERE p.PasswordId = @PasswordId";
            var result = await _dbConnection.QueryAsync<Password, User, Password>(query,
                (password, user) => { password.CreatedBy = user.Email; return password; },
                new { PasswordId = passwordId }, splitOn: "UserId");
            var password = result.FirstOrDefault();
            if (password != null)
            {
                password.EncryptedPassword = _dataProtectionHelper.Decrypt(password.EncryptedPassword);
            }
            return password;
        }

        public async Task<Password> GetPasswordByIdAsync(int passwordId, int userId)
        {
            var sql = @"
                    SELECT p.PasswordId, p.PasswordName, p.EncryptedPassword, p.UserId, u.Email as CreatedBy
                    FROM Password p
                    JOIN [User] u ON p.UserId = u.UserId
                    WHERE p.PasswordId = @PasswordId;

                    SELECT l.LabelId, l.PasswordId, l.UserId, l.LabelText 
                    FROM Label l 
                    WHERE l.PasswordId = @PasswordId AND l.UserId = @UserId;
    
                    SELECT pr.RoleId, r.RoleName 
                    FROM PasswordRole pr
                    JOIN Role r ON pr.RoleId = r.RoleId 
                    WHERE pr.PasswordId = @PasswordId;

                    SELECT MIN(ur.RoleId) 
                    FROM UserRole ur 
                    WHERE ur.UserId = @UserId;

                    SELECT MIN(ur.RoleId) 
                    FROM UserRole ur 
                    WHERE ur.UserId = (SELECT UserId FROM Password WHERE PasswordId = @PasswordId);
                ";

            using (var multi = await _dbConnection.QueryMultipleAsync(sql, new { PasswordId = passwordId, UserId = userId }))
            {
                var password = await multi.ReadSingleOrDefaultAsync<Password>();
                if (password != null)
                {
                    password.EncryptedPassword = _dataProtectionHelper.Decrypt(password.EncryptedPassword);
                    password.Labels = (await multi.ReadAsync<Label>()).ToList();
                    password.Roles = (await multi.ReadAsync<Role>()).ToList();

                    // Read role Ids for canEdit calculation
                    var viewerMinRoleId = await multi.ReadSingleOrDefaultAsync<int>();
                    var creatorMinRoleId = await multi.ReadSingleOrDefaultAsync<int>();

                    // Calculate CanEdit
                    password.CanEdit = password.UserId == userId || creatorMinRoleId > viewerMinRoleId;
                }
                return password;
            }
        }

        public async Task<int> GetTotalPasswordCount(int companyId)
        {
            var query = "SELECT COUNT(*) FROM Password p INNER JOIN [User] u ON p.UserId = u.UserId WHERE u.CompanyId = @CompanyId";
            return await _dbConnection.ExecuteScalarAsync<int>(query, new { CompanyId = companyId });
        }
    }
}
