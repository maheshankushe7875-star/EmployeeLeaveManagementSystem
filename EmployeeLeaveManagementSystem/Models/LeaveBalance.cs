namespace EmployeeLeaveManagementSystem.Models
{
    public class LeaveBalance
    {
        public int LeaveBalanceId { get; set; }
        public int EmployeeId { get; set; } 
        public int AnnualLeave { get; set; }
        public int SickLeave { get; set; }
        public int CasualLeave { get; set; }
        public int OtherLeave { get; set; }
        // Navigation Property stores Whole Record
        public Employee Employee { get; set; } = null!;  




    }
}
