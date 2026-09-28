using EmployeeLeaveManagementSystem.Models;

namespace EmployeeLeaveManagementSystem.Repositories
{
    public interface INotificationRepository
    {
        Task<List<Notification>> GetByEmployeeIdAsync(
            int employeeId);

        Task<Notification?> GetByIdAsync(
            int notificationId,
            int employeeId);

        Task<int> GetUnreadCountAsync(
            int employeeId);

        Task AddAsync(
            Notification notification);

        Task SaveChangesAsync();
    }
}