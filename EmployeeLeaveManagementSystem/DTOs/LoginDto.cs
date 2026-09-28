using System.ComponentModel.DataAnnotations;

namespace EmployeeLeaveManagementSystem.DTOs
{
    public class LoginDto
    {
        [Required(ErrorMessage = "Email is Required.")]
        [EmailAddress(ErrorMessage = "Enter a valid email address")]
        public string Email { get; set; } = string.Empty;
        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; } = string.Empty;

    }
}
