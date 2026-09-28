namespace EmployeeLeaveManagementSystem.Models
{
    public class LeaveRequest
    {
        public int LeaveRequestId { get; set; }
        public int EmployeeId { get; set; } // Foreign Key = store only id 
        public string LeaveType { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending";
        public string? AdminRemark { get; set; }
        public DateTime DateSubmitted { get; set; } = DateTime.UtcNow;
        
        // Navigation Property
        public Employee Employee { get; set; } = null!;


    }
}
