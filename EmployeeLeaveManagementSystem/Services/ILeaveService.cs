using EmployeeLeaveManagementSystem.DTOs;
using EmployeeLeaveManagementSystem.Models;

namespace EmployeeLeaveManagementSystem.Services
{
    public interface ILeaveService
    {
        Task<LeaveRequest> ApplyLeaveAsync(
            int employeeId,
            ApplyLeaveDto dto);

        Task<LeaveRequest> ApproveLeaveAsync(
            int leaveRequestId,
            string remarks);

        Task<LeaveRequest> RejectLeaveAsync(
            int leaveRequestId,
            string remarks);
    }
}