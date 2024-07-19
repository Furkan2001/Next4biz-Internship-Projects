namespace PasswordManagementSystem.Dtos
{
    public class UserDto
    {
        public int UserId { get; set; } // Kullanıcının ID'si (varsa)
        public string? Name { get; set; } // Kullanıcının adı
        public string? Password { get; set; }
        public string? Email { get; set; } // Kullanıcının e-posta adresi
        public string? Company { get; set; } // Kullanıcının şirket adı
        public int CompanyId { get; set; }
        public List<int>? Roles { get; set; } // Kullanıcının rollerinin ID'leri

    }
}
