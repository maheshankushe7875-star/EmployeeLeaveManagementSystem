using EmployeeLeaveManagementSystem.Data;
using EmployeeLeaveManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using EmployeeLeaveManagementSystem.Constants;

namespace EmployeeLeaveManagementSystem.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly AppDbContext _context;

        public EmployeeRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Employee?> GetByEmailAsync(
            string email)
        {
            return await _context.Employees
                .FirstOrDefaultAsync(e =>
                    e.Email == email);
        }

        public async Task<Employee?> GetByIdAsync(
            int employeeId)
        {
            return await _context.Employees
                .FirstOrDefaultAsync(e =>
                    e.EmployeeId == employeeId);
        }

        public async Task<List<LeaveRequest>> GetLeaveRequestsAsync(
            int employeeId)
        {
            return await _context.LeaveRequests
                .Where(l =>
                    l.EmployeeId == employeeId)
                .OrderByDescending(l =>
                    l.DateSubmitted)
                .ToListAsync();
        }

        public async Task<LeaveBalance?> GetLeaveBalanceAsync(
            int employeeId)
        {
            return await _context.LeaveBalances
                .FirstOrDefaultAsync(b =>
                    b.EmployeeId == employeeId);
        }

        public async Task<int> GetEmployeeCountAsync()
        {
            return await _context.Employees
                .CountAsync(e => e.Role == "Employee");
        }

        // 👇 ADDED NEW METHODS HERE
        public async Task<int> GetTotalLeaveRequestsAsync(
            int employeeId)
        {
            return await _context.LeaveRequests
                .CountAsync(l =>
                    l.EmployeeId == employeeId);
        }

        public async Task<int> GetLeaveRequestsCountByStatusAsync(
            int employeeId,
            string status)
        {
            return await _context.LeaveRequests
                .CountAsync(l =>
                    l.EmployeeId == employeeId &&
                    l.Status == status);
        }

        public async Task AddAsync(Employee employee)
        {
            await _context.Employees.AddAsync(employee);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}