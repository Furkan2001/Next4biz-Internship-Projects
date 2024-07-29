using PasswordManagementSystem.Core.Models;

namespace PasswordManagementSystem.Core.ViewModels
{
    public class CompanyLogsViewModel
    {
        public List<Log>? Logs { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}
