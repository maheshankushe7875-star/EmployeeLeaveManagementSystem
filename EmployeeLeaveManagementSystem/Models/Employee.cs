namespace EmployeeLeaveManagementSystem.Models
{
    public class Employee
    {

        public int EmployeeId { get; set; }

        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string Designation { get; set; } = string.Empty;
        public string Role { get; set; } = "Employee";
           
        // Navigation Properties
        public LeaveBalance? LeaveBalance { get; set; }

        public ICollection<LeaveRequest> LeaveRequest { get; set; }
            = new List<LeaveRequest>();
        public ICollection<Notification> Notifications { get; set; }
            = new List<Notification>();




    }
}
