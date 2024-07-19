using System.Collections.Generic;

namespace PasswordManagementSystem.Models
{
    public class Role
    {
        public int RoleId { get; set; }
        public string? RoleName { get; set; }
        public int CompanyId { get; set; }
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
