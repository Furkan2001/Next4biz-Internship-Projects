namespace PasswordManagementSystem.ViewModels
{
    public class PasswordDetailViewModel
    {
        public int PasswordId { get; set; }
        public string PasswordName { get; set; }
        public List<string> Labels { get; set; } // Ensure Labels property is a list of strings
        public string CreatorEmail { get; set; }
    }
}
