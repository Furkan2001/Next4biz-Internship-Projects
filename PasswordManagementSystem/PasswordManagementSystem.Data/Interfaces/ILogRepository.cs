using PasswordManagementSystem.Core.Models;

namespace PasswordManagementSystem.Data.Interfaces
{
    public interface ILogRepository : IRepository<Log>
    {
        Task<IEnumerable<Log>> GetLogsByCompanyId(int companyId);
        Task<IEnumerable<Log>> GetLogsByCompanyId(int companyId, int page, int pageSize);
        Task<int> GetTotalLogCount(int companyId);
    }
}
