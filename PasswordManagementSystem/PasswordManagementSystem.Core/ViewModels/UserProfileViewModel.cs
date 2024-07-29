using PasswordManagementSystem.Core.Models;

namespace PasswordManagementSystem.Core.ViewModels
{
    public class UserProfileViewModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string CompanyName { get; set; }
        public List<Role> Roles { get; set; }
    }
}
