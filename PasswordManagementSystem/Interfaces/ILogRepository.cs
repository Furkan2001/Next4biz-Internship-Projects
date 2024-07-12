using PasswordManagementSystem.Models;

namespace PasswordManagementSystem.Interfaces
{
    public interface ILogRepository : IRepository<Log>
    {
        Task<IEnumerable<Log>> GetLogsByCompanyId(int companyId);
    }
}
