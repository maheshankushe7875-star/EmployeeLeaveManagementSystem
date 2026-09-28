using EmployeeLeaveManagementSystem.Models;

namespace EmployeeLeaveManagementSystem.Repositories
{
    public interface ILeaveRepository
    {
        Task<LeaveRequest?> GetByIdAsync(
            int leaveRequestId);

        Task<bool> HasOverlappingLeaveAsync(
            int employeeId,
            DateTime startDate,
            DateTime endDate);

        Task<List<LeaveRequest>> GetPendingLeavesAsync();

        Task<List<LeaveRequest>> GetLeaveHistoryAsync();

        Task<int> GetTotalLeaveRequestsAsync();

        Task<int> GetLeaveRequestsCountByStatusAsync(
            string status);

        Task AddAsync(
            LeaveRequest leaveRequest);

        Task SaveChangesAsync();
    }
}