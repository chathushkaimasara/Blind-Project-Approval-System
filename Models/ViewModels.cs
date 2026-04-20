using System.ComponentModel.DataAnnotations;

namespace BlindMatchPAS.Models
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Register As Role")]
        public string Role { get; set; } = string.Empty;
    }

    public class UserListViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }

    public class CreateUserViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Role")]
        public string Role { get; set; } = string.Empty;
    }

    public class EditUserViewModel
    {
        public string Id { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        public string Role { get; set; } = string.Empty;
    }

    public class MatchListViewModel
    {
        public int ProjectId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string StudentEmail { get; set; } = string.Empty;
        public string SupervisorEmail { get; set; } = string.Empty;
        public string ResearchArea { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }

    public class ReassignProjectViewModel
    {
        public int ProjectId { get; set; }
        public string ProjectTitle { get; set; } = string.Empty;
        public string CurrentSupervisorId { get; set; } = string.Empty;
        public string CurrentSupervisorEmail { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Please select a new supervisor.")]
        [Display(Name = "New Supervisor")]
        public string NewSupervisorId { get; set; } = string.Empty;
    }
}
