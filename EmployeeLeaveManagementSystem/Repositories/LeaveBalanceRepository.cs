using EmployeeLeaveManagementSystem.Data;
using EmployeeLeaveManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeLeaveManagementSystem.Repositories
{
    public class LeaveBalanceRepository : ILeaveBalanceRepository
    {
        private readonly AppDbContext _context;

        public LeaveBalanceRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<LeaveBalance?> GetByEmployeeIdAsync(
            int employeeId)
        {
            return await _context.LeaveBalances
                .FirstOrDefaultAsync(b =>
                    b.EmployeeId == employeeId);
        }

        public async Task AddAsync(
            LeaveBalance leaveBalance)
        {
            await _context.LeaveBalances.AddAsync(
                leaveBalance);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}