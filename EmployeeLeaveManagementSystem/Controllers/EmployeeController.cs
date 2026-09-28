
using EmployeeLeaveManagementSystem.DTOs;
using EmployeeLeaveManagementSystem.Models;
using EmployeeLeaveManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Security.Claims;
using EmployeeLeaveManagementSystem.Repositories;
using EmployeeLeaveManagementSystem.Constants;

namespace EmployeeLeaveManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Employee")]
    public class EmployeeController : ControllerBase
    {
        
        private readonly ILeaveService _leaveService;
        private readonly INotificationRepository _notificationRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ILogger<EmployeeController> _logger;
        

        public EmployeeController(
            
            ILeaveService leaveService,
                INotificationRepository notificationRepository,
                    IEmployeeRepository employeeRepository,
            ILogger<EmployeeController> logger)
            
        {
            
            _leaveService = leaveService;
            _notificationRepository = notificationRepository;
            _employeeRepository = employeeRepository;
            _logger = logger;

        }

        [HttpGet("leave-balance")]
        public async Task<IActionResult> GetLeaveBalance()
        {
            var employeeId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var balance =
                await _employeeRepository
                    .GetLeaveBalanceAsync(employeeId);

            if (balance == null)
            {
                return NotFound("Leave balance not found.");
            }

            return Ok(new
            {
                annualLeave = balance.AnnualLeave,
                sickLeave = balance.SickLeave,
                casualLeave = balance.CasualLeave,
                otherLeave = balance.OtherLeave
            });
        }
        // ✅ Closes GetLeaveBalance()


        //[HttpPost("apply-leave")]
        //public async Task<IActionResult> ApplyLeave(ApplyLeaveDto dto)
        //{
        //    // Get EmployeeId from JWT token
        //    var employeeIdClaim = User.FindFirstValue(
        //        ClaimTypes.NameIdentifier);

        //    if (employeeIdClaim == null)
        //    {
        //        return Unauthorized("Employee ID not found in token.");
        //    }

        //    int employeeId = int.Parse(employeeIdClaim);

        //    // Basic validation
        //    if (string.IsNullOrWhiteSpace(dto.LeaveType))
        //    {
        //        return BadRequest("Leave type is required.");
        //    }

        //    if (string.IsNullOrWhiteSpace(dto.Reason))
        //    {
        //        return BadRequest("Reason is required.");
        //    }

        //    // Date validation
        //    if (dto.StartDate.Date > dto.EndDate.Date)
        //    {
        //        return BadRequest(
        //            "Start date cannot be greater than end date.");
        //    }

        //    if (dto.StartDate.Date < DateTime.Today)
        //    {
        //        return BadRequest(
        //            "Leave start date cannot be in the past.");
        //    }

        //    // Calculate number of leave days
        //    int leaveDays =
        //        (dto.EndDate.Date - dto.StartDate.Date).Days + 1;

        //    // Check overlapping leave
        //    bool hasOverlappingLeave = await _context.LeaveRequests
        //        .AnyAsync(l =>
        //            l.EmployeeId == employeeId &&
        //            l.Status != "Rejected" &&
        //            dto.StartDate.Date <= l.EndDate.Date &&
        //            dto.EndDate.Date >= l.StartDate.Date);

        //    if (hasOverlappingLeave)
        //    {
        //        return BadRequest(
        //            "You already have a leave request for these dates.");
        //    }

        //    // Get employee's leave balance
        //    var leaveBalance = await _context.LeaveBalances
        //        .FirstOrDefaultAsync(lb => lb.EmployeeId == employeeId);

        //    if (leaveBalance == null)
        //    {
        //        return NotFound("Leave balance not found.");
        //    }

        //    // Check leave balance
        //    int availableLeaves = dto.LeaveType.ToLower() switch
        //    {
        //        "annual" => leaveBalance.AnnualLeave,
        //        "sick" => leaveBalance.SickLeave,
        //        "casual" => leaveBalance.CasualLeave,
        //        "other" => leaveBalance.OtherLeave,
        //        _ => -1
        //    };

        //    if (availableLeaves == -1)
        //    {
        //        return BadRequest(
        //            "Invalid leave type. Use Annual, Sick, Casual, or Other.");
        //    }

        //    if (leaveDays > availableLeaves)
        //    {
        //        return BadRequest(
        //            $"Insufficient {dto.LeaveType} leave balance.");
        //    }

        //    // Create leave request
        //    var leaveRequest = new LeaveRequest
        //    {
        //        EmployeeId = employeeId,
        //        LeaveType = dto.LeaveType,
        //        StartDate = dto.StartDate.Date,
        //        EndDate = dto.EndDate.Date,
        //        Reason = dto.Reason,
        //        Status = "Pending",
        //        DateSubmitted = DateTime.UtcNow
        //    };

        //    _context.LeaveRequests.Add(leaveRequest);

        //    await _context.SaveChangesAsync();
        //    var notification = new Notification
        //    {
        //        EmployeeId = employeeId,
        //        Message = "Your leave request has been submitted successfully.",
        //        IsRead = false,
        //        CreatedAt = DateTime.UtcNow
        //    };

        //    _context.Notifications.Add(notification);

        //    await _context.SaveChangesAsync();
        //    _logger.LogInformation(
        //        "Leave submitted. LeaveRequestId: {LeaveRequestId}, EmployeeId: {EmployeeId}, LeaveType: {LeaveType}",
        //         leaveRequest.LeaveRequestId,
        //         employeeId,
        //         leaveRequest.LeaveType);


        //    return Ok(new
        //    {
        //        message = "Leave request submitted successfully.",
        //        leaveRequestId = leaveRequest.LeaveRequestId,
        //        employeeId = employeeId,
        //        leaveType = leaveRequest.LeaveType,
        //        startDate = leaveRequest.StartDate,
        //        endDate = leaveRequest.EndDate,
        //        totalDays = leaveDays,
        //        status = leaveRequest.Status
        //    });
        //}
        [HttpPost("apply-leave")]
        public async Task<IActionResult> ApplyLeave(ApplyLeaveDto dto)
        {
            var employeeIdClaim =
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(employeeIdClaim))
            {
                return Unauthorized("Employee ID not found in token.");
            }

            int employeeId = int.Parse(employeeIdClaim);

            var leaveRequest =
                await _leaveService.ApplyLeaveAsync(employeeId, dto);

            await _notificationRepository.AddAsync(new Notification
            {
                EmployeeId = employeeId,
                Message = "Your leave request has been submitted successfully.",
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            });

            await _notificationRepository.SaveChangesAsync();

            return Ok(new
            {
                message = "Leave request submitted successfully.",
                leaveRequestId = leaveRequest.LeaveRequestId,
                employeeId = leaveRequest.EmployeeId,
                leaveType = leaveRequest.LeaveType,
                startDate = leaveRequest.StartDate,
                endDate = leaveRequest.EndDate,
                status = leaveRequest.Status
            });
        }
        // GET: api/Employee/leave-requests
        [HttpGet("leave-requests")]
        public async Task<IActionResult> GetLeaveRequests()
        {
            var employeeId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var leaveRequests =
                await _employeeRepository
                    .GetLeaveRequestsAsync(employeeId);

            return Ok(leaveRequests.Select(l => new
            {
                leaveRequestId = l.LeaveRequestId,
                leaveType = l.LeaveType,
                startDate = l.StartDate,
                endDate = l.EndDate,
                totalDays = (l.EndDate.Date - l.StartDate.Date).Days + 1,
                reason = l.Reason,
                status = l.Status,
                adminRemarks = l.AdminRemark,
                dateSubmitted = l.DateSubmitted
            }));
        }
        // GET: api/Employee/notifications
        [HttpGet("notifications")]
        public async Task<IActionResult> GetNotifications()
        {
            var employeeId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var notifications =
                await _notificationRepository
                    .GetByEmployeeIdAsync(employeeId);

            return Ok(notifications.Select(n => new
            {
                notificationId = n.NotificationId,
                message = n.Message,
                isRead = n.IsRead,
                createdAt = n.CreatedAt
            }));
        }
        [HttpPut("notifications/{id}/read")]
        public async Task<IActionResult> MarkNotificationAsRead(int id)
        {
            var employeeId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var notification =
                await _notificationRepository
                    .GetByIdAsync(id, employeeId);

            if (notification == null)
            {
                return NotFound("Notification not found.");
            }

            notification.IsRead = true;

            await _notificationRepository.SaveChangesAsync();

            return Ok(new
            {
                message = "Notification marked as read successfully.",
                notificationId = notification.NotificationId,
                isRead = notification.IsRead
            });
        }
        [HttpGet("notifications/unread-count")]
        public async Task<IActionResult> GetUnreadNotificationCount()
        {
            var employeeId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var unreadCount =
                await _notificationRepository
                    .GetUnreadCountAsync(employeeId);

            return Ok(new
            {
                unreadCount
            });
        }
        // GET: api/Employee/dashboard

        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboard()
        {
            var employeeId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var employee =
                await _employeeRepository.GetByIdAsync(employeeId);

            if (employee == null)
            {
                return NotFound("Employee not found.");
            }

            var balance =
                await _employeeRepository.GetLeaveBalanceAsync(employeeId);

            if (balance == null)
            {
                return NotFound("Leave balance not found.");
            }

            var total =
                await _employeeRepository
                    .GetTotalLeaveRequestsAsync(employeeId);

            var pending =
                await _employeeRepository
                    .GetLeaveRequestsCountByStatusAsync(
                        employeeId,
                        "Pending");

            var approved =
                await _employeeRepository
                    .GetLeaveRequestsCountByStatusAsync(
                        employeeId,
                        "Approved");

            var rejected =
                await _employeeRepository
                    .GetLeaveRequestsCountByStatusAsync(
                        employeeId,
                        "Rejected");

            return Ok(new
            {
                employee = new
                {
                    employeeId = employee.EmployeeId,
                    name = employee.Name,
                    department = employee.Department,
                    designation = employee.Designation
                },

                leaveBalance = new
                {
                    annualLeave = balance.AnnualLeave,
                    sickLeave = balance.SickLeave,
                    casualLeave = balance.CasualLeave,
                    otherLeave = balance.OtherLeave
                },

                summary = new
                {
                    total,
                    pending,
                    approved,
                    rejected
                }
            });
        }
    }
}