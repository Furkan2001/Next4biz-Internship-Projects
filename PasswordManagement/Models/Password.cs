namespace PasswordManagementSystem.Models
{
    public class Password
    {
        public int PasswordId { get; set; }
        public int UserId { get; set; }
        public string? EncryptedPassword { get; set; }
        public string? PasswordName { get; set; }
    }

}
