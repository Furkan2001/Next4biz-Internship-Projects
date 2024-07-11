using PasswordManagementSystem.Models;
using System.Collections.Generic;

namespace PasswordManagementSystem.ViewModels
{
    public class UserDetailsViewModel
    {
        public User User { get; set; }
        public IEnumerable<Role> Roles { get; set; }
        public Company Company { get; set; }
    }
}
