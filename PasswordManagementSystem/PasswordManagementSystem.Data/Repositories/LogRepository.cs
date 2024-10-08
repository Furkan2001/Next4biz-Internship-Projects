﻿using Dapper;
using PasswordManagementSystem.Data.Interfaces;
using PasswordManagementSystem.Core.Models;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace PasswordManagementSystem.Data.Repositories
{
    public class LogRepository : ILogRepository
    {
        private readonly IDbConnection _db;

        public LogRepository(IDbConnection db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Log>> GetAll()
        {
            return await _db.QueryAsync<Log>("SELECT * FROM Logs");
        }

        public async Task<Log> GetById(int id)
        {
            return await _db.QueryFirstOrDefaultAsync<Log>("SELECT * FROM Logs WHERE LogId = @Id", new { Id = id });
        }

        public async Task<IEnumerable<Log>> GetLogsByCompanyId(int companyId)
        {
            var sql = @"
                SELECT l.* 
                FROM [Log] l
                INNER JOIN [User] u ON l.UserId = u.UserId
                WHERE u.CompanyId = @CompanyId";

            return await _db.QueryAsync<Log>(sql, new { CompanyId = companyId });
        }


        public async Task Add(Log entity)
        {
            var sql = "INSERT INTO Logs (UserId, Action, Date) VALUES (@UserId, @Action, @Date)";
            await _db.ExecuteAsync(sql, entity);
        }

        public async Task Update(Log entity)
        {
            var sql = "UPDATE Logs SET UserId = @UserId, Action = @Action, Date = @Date WHERE LogId = @LogId";
            await _db.ExecuteAsync(sql, entity);
        }

        public async Task Delete(int id)
        {
            var sql = "DELETE FROM Logs WHERE LogId = @Id";
            await _db.ExecuteAsync(sql, new { Id = id });
        }

        public async Task<IEnumerable<Log>> GetLogsByCompanyId(int companyId, int page, int pageSize)
        {
            var query = @"SELECT l.*
                      FROM Log l
                      INNER JOIN [User] u ON l.UserId = u.UserId
                      WHERE u.CompanyId = @CompanyId
                      ORDER BY l.Date DESC
                      OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY"
            ;

            return await _db.QueryAsync<Log>(query, new
            {
                CompanyId = companyId,
                Offset = (page - 1) * pageSize,
                PageSize = pageSize
            });
        }

        public async Task<int> GetTotalLogCount(int companyId)
        {
            var query = @"SELECT COUNT(*)
                      FROM Log l
                      INNER JOIN [User] u ON l.UserId = u.UserId
                      WHERE u.CompanyId = @CompanyId"
            ;

            return await _db.ExecuteScalarAsync<int>(query, new { CompanyId = companyId });
        }
    }
}
