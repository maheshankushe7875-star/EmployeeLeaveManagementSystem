namespace EmployeeLeaveManagementSystem.Models
{
    public class Notification
    {
        public int NotificationId { get; set; }

        public int EmployeeId { get; set; }

        public string Message { get; set; } = string.Empty;

        public bool IsRead { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Employee Employee { get; set; } = null!;
    }
}