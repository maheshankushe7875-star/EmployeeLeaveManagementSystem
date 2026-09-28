using System.ComponentModel.DataAnnotations;

namespace EmployeeLeaveManagementSystem.DTOs
{
    public class RegisterDto   // DTO = it is used to define date provided by client 
    {
        [Required(ErrorMessage = "Name is Required.")]   // this aree attribute Validation = Reuired min lenghtht validity
        public string Name { get; set; } = string.Empty;
        [Required(ErrorMessage = "Email is Required")]
        [EmailAddress(ErrorMessage = "Enter a valid email address")]
        public string Email { get; set; } = string.Empty;
        [Required(ErrorMessage = "Password is Required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters.")]
        public string Password { get; set; } = string.Empty;
        [Required(ErrorMessage = "Department is Required")]
        public string Department { get; set; } = string.Empty;
        [Required(ErrorMessage = "Designation is Required")]
        public string Designation { get; set; } = string.Empty;
    }
}
