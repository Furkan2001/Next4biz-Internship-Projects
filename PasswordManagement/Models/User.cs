using System.Collections.Generic;

namespace PasswordManagementSystem.Models
{
    public class User
    {
        public int UserId { get; set; }
        public int CompanyId { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? Name { get; set; }
        public ICollection<Role> Roles { get; set; } = new List<Role>();
    }
}
