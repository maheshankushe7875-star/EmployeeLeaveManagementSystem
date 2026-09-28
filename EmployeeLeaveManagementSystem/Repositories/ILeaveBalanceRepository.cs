using EmployeeLeaveManagementSystem.Models;

namespace EmployeeLeaveManagementSystem.Repositories
{
    public interface ILeaveBalanceRepository
    {
        Task<LeaveBalance?> GetByEmployeeIdAsync(int employeeId);

        Task AddAsync(LeaveBalance leaveBalance);

        Task SaveChangesAsync();
    }
}