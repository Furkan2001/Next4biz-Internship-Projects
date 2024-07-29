using PasswordManagementSystem.Core.Models;
using System.Collections.Generic;

namespace PasswordManagementSystem.Core.ViewModels
{
    public class UserDetailsViewModel
    {
        public User User { get; set; }
        public IEnumerable<Role> Roles { get; set; }
        public Company Company { get; set; }
    }
}
