using PasswordManagementSystem.Core.Models;

namespace PasswordManagementSystem.Business.Services
{
    public interface ICompanyService
    {
        Task<IEnumerable<Company>> GetAllCompanies();
        Task<Company> GetCompanyById(int id);
        Task CreateCompany(Company company);
        Task UpdateCompany(Company company);
        Task DeleteCompany(int id);
    }
}
