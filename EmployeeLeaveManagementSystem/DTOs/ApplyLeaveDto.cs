using System.ComponentModel.DataAnnotations;

namespace EmployeeLeaveManagementSystem.DTOs
{
    public class ApplyLeaveDto
    {
        [Required(ErrorMessage = "Leave type is required.")]    // Validation
        public string LeaveType { get; set; } = string.Empty;
        [Required(ErrorMessage = "Start date is Required.")]
        public DateTime StartDate  { get; set; }
        [Required(ErrorMessage = "End date is Required.")]
        public DateTime EndDate { get; set; }
        [Required(ErrorMessage = "Reasone is required.")]
        [MinLength(3, ErrorMessage = "Reasone must be at least 3 characters.")]
        public string Reason { get; set; } = string.Empty;
        public DateTime DateSubmitted { get; set; }
    }
}
