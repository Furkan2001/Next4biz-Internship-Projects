using Dapper;
using PasswordManagementSystem.Interfaces;
using PasswordManagementSystem.Models;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace PasswordManagementSystem.Repositories
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly IDbConnection _db;

        public CompanyRepository(IDbConnection db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Company>> GetAll()
        {
            return await _db.QueryAsync<Company>("SELECT * FROM Companies");
        }

        public async Task<Company> GetById(int id)
        {
            return await _db.QueryFirstOrDefaultAsync<Company>("SELECT * FROM Companies WHERE CompanyId = @Id", new { Id = id });
        }

        public async Task Add(Company entity)
        {
            var sql = "INSERT INTO Companies (CompanyName, Domain) VALUES (@CompanyName, @Domain)";
            await _db.ExecuteAsync(sql, entity);
        }

        public async Task Update(Company entity)
        {
            var sql = "UPDATE Companies SET CompanyName = @CompanyName, Domain = @Domain WHERE CompanyId = @CompanyId";
            await _db.ExecuteAsync(sql, entity);
        }

        public async Task Delete(int id)
        {
            var sql = "DELETE FROM Companies WHERE CompanyId = @Id";
            await _db.ExecuteAsync(sql, new { Id = id });
        }
    }
}
