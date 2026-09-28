using EmployeeLeaveManagementSystem.Data;
using EmployeeLeaveManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using EmployeeLeaveManagementSystem.Constants;

namespace EmployeeLeaveManagementSystem.Repositories
{
    public class LeaveRepository : ILeaveRepository
    {
        private readonly AppDbContext _context;

        public LeaveRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<LeaveRequest?> GetByIdAsync(
            int leaveRequestId)
        {
            return await _context.LeaveRequests
                .FirstOrDefaultAsync(l =>
                    l.LeaveRequestId == leaveRequestId);
        }

        public async Task<bool> HasOverlappingLeaveAsync(
            int employeeId,
            DateTime startDate,
            DateTime endDate)
        {
            return await _context.LeaveRequests
                .AnyAsync(l =>
                    l.EmployeeId == employeeId &&
                    l.Status != LeaveConstants.Rejected &&
                    startDate.Date <= l.EndDate.Date &&
                    endDate.Date >= l.StartDate.Date);
        }
        public async Task<List<LeaveRequest>> GetPendingLeavesAsync()
        {
            return await _context.LeaveRequests
                .Include(l => l.Employee)
                .Where(l => l.Status == "Pending")
                .OrderByDescending(l => l.DateSubmitted)
                .ToListAsync();
        }

        public async Task<List<LeaveRequest>> GetLeaveHistoryAsync()
        {
            return await _context.LeaveRequests
                .Include(l => l.Employee)
                .Where(l =>
                    l.Status == LeaveConstants.Approved ||
                    l.Status == LeaveConstants.Rejected)
                .OrderByDescending(l => l.DateSubmitted)
                .ToListAsync();
        }

        public async Task<int> GetTotalLeaveRequestsAsync()
        {
            return await _context.LeaveRequests
                .CountAsync();
        }

        public async Task<int> GetLeaveRequestsCountByStatusAsync(
            string status)
        {
            return await _context.LeaveRequests
                .CountAsync(l => l.Status == status);
        }
        public async Task AddAsync(
            LeaveRequest leaveRequest)
        {
            await _context.LeaveRequests.AddAsync(
                leaveRequest);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}