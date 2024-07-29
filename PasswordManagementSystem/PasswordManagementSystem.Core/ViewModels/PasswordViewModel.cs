using PasswordManagementSystem.Core.Models;
using System.Collections.Generic;

namespace PasswordManagementSystem.Core.ViewModels
{
    public class PasswordViewModel
    {
        public int PasswordId { get; set; }
        public string? PasswordName { get; set; }
        public string? EncryptedPassword { get; set; }
        public int UserId { get; set; }
        public string? CreatedBy { get; set; }
        public List<LabelViewModel>? Labels { get; set; }
        public List<RoleViewModel>? Roles { get; set; }
        public Password Password { get; set; }
        public List<int>? RoleIds { get; set; }
        public string? Label { get; set; }
        public List<PasswordViewModel>? Passwords { get; set; } // Corrected this line
        public bool CanEdit { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
    }
}
