using PasswordManagementSystem.Interfaces;
using PasswordManagementSystem.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PasswordManagementSystem.Services
{
    public class CompanyService
    {
        private readonly ICompanyRepository _companyRepository;

        public CompanyService(ICompanyRepository companyRepository)
        {
            _companyRepository = companyRepository;
        }

        public async Task<IEnumerable<Company>> GetAllCompanies()
        {
            return await _companyRepository.GetAll();
        }

        public async Task<Company> GetCompanyById(int id)
        {
            return await _companyRepository.GetById(id);
        }

        public async Task CreateCompany(Company company)
        {
            await _companyRepository.Add(company);
        }

        public async Task UpdateCompany(Company company)
        {
            await _companyRepository.Update(company);
        }

        public async Task DeleteCompany(int id)
        {
            await _companyRepository.Delete(id);
        }
    }
}
