namespace PasswordManagementSystem.Models
{
    public class PasswordDto
    {
        public int PasswordId { get; set; }
        public int UserId { get; set; }
        public string? PasswordName { get; set; }
        public string? EncryptedPassword { get; set; }
        public string? LabelName { get; set; }
    }
}
