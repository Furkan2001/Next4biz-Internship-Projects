namespace PasswordManagementSystem.Models
{
    public class PasswordRole
    {
        public int PasswordRoleId { get; set; }
        public int PasswordId { get; set; }
        public int GroupId { get; set; }
    }
}
