namespace PasswordManagementSystem.Models
{
    public class Label
    {
        public int LabelId { get; set; }
        public int UserId { get; set; }
        public int PasswordId { get; set; }
        public string? LabelText { get; set; }
    }
}
