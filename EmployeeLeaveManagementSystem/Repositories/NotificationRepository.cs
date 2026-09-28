using EmployeeLeaveManagementSystem.Data;
using EmployeeLeaveManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeLeaveManagementSystem.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly AppDbContext _context;

        public NotificationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Notification>> GetByEmployeeIdAsync(
            int employeeId)
        {
            return await _context.Notifications
                .Where(n => n.EmployeeId == employeeId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task<Notification?> GetByIdAsync(
            int notificationId,
            int employeeId)
        {
            return await _context.Notifications
                .FirstOrDefaultAsync(n =>
                    n.NotificationId == notificationId &&
                    n.EmployeeId == employeeId);
        }

        public async Task<int> GetUnreadCountAsync(
            int employeeId)
        {
            return await _context.Notifications
                .CountAsync(n =>
                    n.EmployeeId == employeeId &&
                    !n.IsRead);
        }

        public async Task AddAsync(
            Notification notification)
        {
            await _context.Notifications.AddAsync(notification);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}