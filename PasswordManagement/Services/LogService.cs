using PasswordManagementSystem.Interfaces;
using PasswordManagementSystem.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PasswordManagementSystem.Services
{
    public class LogService
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
    }
}
