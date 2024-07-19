namespace PasswordManagementSystem.Models
{
    public class Password
    {
        public int PasswordId { get; set; }
        public string? PasswordName { get; set; }
        public string? EncryptedPassword { get; set; }
        public int UserId { get; set; }
        public string? CreatedBy { get; set; } // CreatedBy alanı
        public User? User { get; set; }
        public List<Label>? Labels { get; set; }
        public List<Role>? Roles { get; set; }
        public bool CanEdit { get; set; } // Şifre düzenleme izni
    }
}
