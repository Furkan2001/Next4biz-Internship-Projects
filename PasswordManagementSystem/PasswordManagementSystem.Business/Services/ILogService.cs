using PasswordManagementSystem.Core.Models;

namespace PasswordManagementSystem.Business.Services
{
    public interface ILogService
    {
        Task<IEnumerable<Log>> GetAllLogs();
        Task<Log> GetLogById(int id);
        Task<IEnumerable<Log>> GetLogsByCompanyId(int companyId);
        Task CreateLog(Log log);
        Task UpdateLog(Log log);
        Task DeleteLog(int id);
        Task<IEnumerable<Log>> GetLogsByCompanyId(int companyId, int page, int pageSize);
        Task<int> GetTotalLogCount(int companyId);
    }
}
