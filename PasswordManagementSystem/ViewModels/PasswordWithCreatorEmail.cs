using PasswordManagementSystem.Models;

namespace PasswordManagementSystem.ViewModels
{
    public class PasswordWithCreatorEmail
    {
        public int PasswordId { get; set; }
        public string? PasswordName { get; set; }
        public string? EncryptedPassword { get; set; }
        public int UserId { get; set; }
        public string? CreatedBy { get; set; } // Email bilgisi
        public bool CanEdit { get; set; } // Şifre düzenleme izni
        public List<Label> Labels { get; set; } = new List<Label>();
    }
}
