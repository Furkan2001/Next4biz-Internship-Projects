namespace PasswordManagementSystem.Models
{
    public class Log
    {
        public int LogId { get; set; }
        public int UserId { get; set; }
        public string? Action { get; set; }
        public DateTime Date { get; set; } // Yeni eklenen sütun
    }
}
