using Microsoft.OpenApi.MicrosoftExtensions;
using System.ComponentModel.DataAnnotations;

namespace EmployeeLeaveManagementSystem.DTOs
{
    public class AdminRemarksDto
    {
        [Required(ErrorMessage = "Remarks are required.")]
        [MinLength(3, ErrorMessage = "Remarks must be at least 3 characters.")]
        public string? Remarks { get; set; }
    }
}