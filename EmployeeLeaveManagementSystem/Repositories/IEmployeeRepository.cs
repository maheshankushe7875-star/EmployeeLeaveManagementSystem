using EmployeeLeaveManagementSystem.Models;

namespace EmployeeLeaveManagementSystem.Repositories
{
    public interface IEmployeeRepository
    {
        Task<Employee?> GetByEmailAsync(string email);

        Task<Employee?> GetByIdAsync(int employeeId);

        Task<List<LeaveRequest>> GetLeaveRequestsAsync(
            int employeeId);

        Task<LeaveBalance?> GetLeaveBalanceAsync(
            int employeeId);

        Task<int> GetEmployeeCountAsync();

        Task<int> GetTotalLeaveRequestsAsync(
            int employeeId);

        Task<int> GetLeaveRequestsCountByStatusAsync(
            int employeeId,
            string status);

        Task AddAsync(Employee employee);

        Task SaveChangesAsync();
    }
}