using PasswordManagementSystem.Data.Interfaces;
using PasswordManagementSystem.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PasswordManagementSystem.Business.Services.Impl
{
    public class LogService : ILogService
    {
        private readonly ILogRepository _logRepository;

        public LogService(ILogRepository logRepository)
        {
            _logRepository = logRepository;
        }

        public async Task<IEnumerable<Log>> GetAllLogs()
        {
            return await _logRepository.GetAll();
        }

        public async Task<Log> GetLogById(int id)
        {
            return await _logRepository.GetById(id);
        }

        public async Task<IEnumerable<Log>> GetLogsByCompanyId(int companyId)
        {
            return await _logRepository.GetLogsByCompanyId(companyId);
        }

        public async Task CreateLog(Log log)
        {
            await _logRepository.Add(log);
        }

        public async Task UpdateLog(Log log)
        {
            await _logRepository.Update(log);
        }

        public async Task DeleteLog(int id)
        {
            await _logRepository.Delete(id);
        }

        public async Task<IEnumerable<Log>> GetLogsByCompanyId(int companyId, int page, int pageSize)
        {
            return await _logRepository.GetLogsByCompanyId(companyId, page, pageSize);
        }

        public async Task<int> GetTotalLogCount(int companyId)
        {
            return await _logRepository.GetTotalLogCount(companyId);
        }
    }
}
