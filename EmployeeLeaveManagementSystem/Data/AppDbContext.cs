
using EmployeeLeaveManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeLeaveManagementSystem.Data
{
    public class AppDbContext:  DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {

        }     

        public DbSet<Employee> Employees { get; set; }
        public DbSet<LeaveRequest> LeaveRequests { get; set; }
        public DbSet<LeaveBalance> LeaveBalances { get; set; }
        public DbSet<Notification> Notifications { get; set; }

    }
}
