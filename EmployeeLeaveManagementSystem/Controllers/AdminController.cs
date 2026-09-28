
using EmployeeLeaveManagementSystem.DTOs;
using EmployeeLeaveManagementSystem.Models;
using EmployeeLeaveManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EmployeeLeaveManagementSystem.Repositories;
using EmployeeLeaveManagementSystem.Constants;

namespace EmployeeLeaveManagementSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        
        private readonly ILeaveService _leaveService;
        private readonly ILeaveRepository _leaveRepository;
        private readonly IEmployeeRepository _employeeRepository;
        IEmployeeRepository employeeRepository;
        private readonly ILogger<AdminController> _logger;

        public AdminController(
            ILeaveService leaveService,
            ILeaveRepository leaveRepository,
             IEmployeeRepository employeeRepository,
            ILogger<AdminController> logger)
        {
           
            _leaveService = leaveService;
            _leaveRepository = leaveRepository;
            _employeeRepository = employeeRepository;
            _logger = logger;
        }

        // GET: api/Admin/pending-leaves
        [HttpGet("pending-leaves")]
        public async Task<IActionResult> GetPendingLeaves()
        {
            var leaves =
                await _leaveRepository.GetPendingLeavesAsync();

            return Ok(leaves.Select(l => new
            {
                leaveRequestId = l.LeaveRequestId,
                employeeId = l.EmployeeId,
                employeeName = l.Employee.Name,
                department = l.Employee.Department,
                designation = l.Employee.Designation,
                leaveType = l.LeaveType,
                startDate = l.StartDate,
                endDate = l.EndDate,
                totalDays =
                    (l.EndDate.Date - l.StartDate.Date).Days + 1,
                reason = l.Reason,
                status = l.Status,
                dateSubmitted = l.DateSubmitted
            }));
        }

        // PUT: api/Admin/approve-leave/{id}
        //[HttpPut("approve-leave/{id}")]
        //public async Task<IActionResult> ApproveLeave(
        //    int id,
        //    AdminRemarksDto dto)
        //{
        //    var leaveRequest = await _context.LeaveRequests
        //        .FirstOrDefaultAsync(l => l.LeaveRequestId == id);

        //    if (leaveRequest == null)
        //    {
        //        return NotFound("Leave request not found.");
        //    }

        //    if (leaveRequest.Status != "Pending")
        //    {
        //        return BadRequest(
        //            "Only pending leave requests can be approved.");
        //    }

        //    var leaveBalance = await _context.LeaveBalances
        //        .FirstOrDefaultAsync(
        //            lb => lb.EmployeeId == leaveRequest.EmployeeId);

        //    if (leaveBalance == null)
        //    {
        //        return NotFound("Employee leave balance not found.");
        //    }

        //    int leaveDays =
        //        (leaveRequest.EndDate.Date - leaveRequest.StartDate.Date).Days + 1;

        //    int availableLeaves = leaveRequest.LeaveType.ToLower() switch
        //    {
        //        "annual" => leaveBalance.AnnualLeave,
        //        "sick" => leaveBalance.SickLeave,
        //        "casual" => leaveBalance.CasualLeave,
        //        "other" => leaveBalance.OtherLeave,
        //        _ => -1
        //    };

        //    if (availableLeaves == -1)
        //    {
        //        return BadRequest("Invalid leave type.");
        //    }

        //    if (leaveDays > availableLeaves)
        //    {
        //        return BadRequest(
        //            $"Insufficient {leaveRequest.LeaveType} leave balance.");
        //    }

        //    // Deduct leave balance
        //    switch (leaveRequest.LeaveType.ToLower())
        //    {
        //        case "annual":
        //            leaveBalance.AnnualLeave -= leaveDays;
        //            break;

        //        case "sick":
        //            leaveBalance.SickLeave -= leaveDays;
        //            break;

        //        case "casual":
        //            leaveBalance.CasualLeave -= leaveDays;
        //            break;

        //        case "other":
        //            leaveBalance.OtherLeave -= leaveDays;
        //            break;
        //    }

        //    leaveRequest.Status = "Approved";
        //    leaveRequest.AdminRemark = dto.Remarks;

        //    await _context.SaveChangesAsync();
        //    var notification = new Notification
        //    {
        //        EmployeeId = leaveRequest.EmployeeId,
        //        Message = "Your leave request has been approved.",
        //        IsRead = false,
        //        CreatedAt = DateTime.UtcNow
        //    };

        //    _context.Notifications.Add(notification);

        //    await _context.SaveChangesAsync();
        //    _logger.LogInformation(
        //         "Leave approved. LeaveRequestId: {LeaveRequestId}, EmployeeId: {EmployeeId}, LeaveDays: {LeaveDays}",
        //         leaveRequest.LeaveRequestId,
        //         leaveRequest.EmployeeId,
        //         leaveDays);


        //    return Ok(new
        //    {
        //        message = "Leave request approved successfully.",
        //        leaveRequestId = leaveRequest.LeaveRequestId,
        //        employeeId = leaveRequest.EmployeeId,
        //        leaveType = leaveRequest.LeaveType,
        //        totalDays = leaveDays,
        //        status = leaveRequest.Status,
        //        adminRemarks = leaveRequest.AdminRemark
        //    });

        //}
        [HttpPut("approve-leave/{id}")]
        public async Task<IActionResult> ApproveLeave(
    int id,
    AdminRemarksDto dto)
        {
            var leaveRequest =
                await _leaveService.ApproveLeaveAsync(
                    id,
                    dto.Remarks);

            return Ok(new
            {
                message = "Leave approved successfully.",
                leaveRequestId = leaveRequest.LeaveRequestId,
                employeeId = leaveRequest.EmployeeId,
                status = leaveRequest.Status,
                adminRemarks = leaveRequest.AdminRemark
            });
        }
        // PUT: api/Admin/reject-leave/{id}
        //[HttpPut("reject-leave/{id}")]
        //public async Task<IActionResult> RejectLeave(
        //    int id,
        //    AdminRemarksDto dto)
        //{
        //    var leaveRequest = await _context.LeaveRequests
        //        .FirstOrDefaultAsync(l => l.LeaveRequestId == id);

        //    if (leaveRequest == null)
        //    {
        //        return NotFound("Leave request not found.");
        //    }

        //    if (leaveRequest.Status != "Pending")
        //    {
        //        return BadRequest(
        //            "Only pending leave requests can be rejected.");
        //    }

        //    leaveRequest.Status = "Rejected";
        //    leaveRequest.AdminRemark = dto.Remarks;

        //    await _context.SaveChangesAsync();
        //    var notification = new Notification
        //    {
        //        EmployeeId = leaveRequest.EmployeeId,
        //        Message = "Your leave request has been approved.",
        //        IsRead = false,
        //        CreatedAt = DateTime.UtcNow
        //    };

        //    _context.Notifications.Add(notification);

        //    await _context.SaveChangesAsync();
        //    _logger.LogInformation(
        //         "Leave rejected. LeaveRequestId: {LeaveRequestId}, EmployeeId: {EmployeeId}",
        //          leaveRequest.LeaveRequestId,
        //          leaveRequest.EmployeeId);


        //    return Ok(new
        //    {
        //        message = "Leave request rejected successfully.",
        //        leaveRequestId = leaveRequest.LeaveRequestId,
        //        employeeId = leaveRequest.EmployeeId,
        //        leaveType = leaveRequest.LeaveType,
        //        status = leaveRequest.Status,
        //        adminRemarks = leaveRequest.AdminRemark
        //    });
        //}
        [HttpPut("reject-leave/{id}")]
        public async Task<IActionResult> RejectLeave(
    int id,
    AdminRemarksDto dto)
        {
            var leaveRequest =
                await _leaveService.RejectLeaveAsync(
                    id,
                    dto.Remarks);

            return Ok(new
            {
                message = "Leave rejected successfully.",
                leaveRequestId = leaveRequest.LeaveRequestId,
                employeeId = leaveRequest.EmployeeId,
                status = leaveRequest.Status,
                adminRemarks = leaveRequest.AdminRemark
            });
        }

        // GET: api/Admin/leave-history
        [HttpGet("leave-history")]
        public async Task<IActionResult> GetLeaveHistory()
        {
            var leaves =
                await _leaveRepository.GetLeaveHistoryAsync();

            return Ok(leaves.Select(l => new
            {
                leaveRequestId = l.LeaveRequestId,
                employeeId = l.EmployeeId,
                employeeName = l.Employee.Name,
                department = l.Employee.Department,
                leaveType = l.LeaveType,
                startDate = l.StartDate,
                endDate = l.EndDate,
                totalDays =
                    (l.EndDate.Date - l.StartDate.Date).Days + 1,
                reason = l.Reason,
                status = l.Status,
                adminRemarks = l.AdminRemark,
                dateSubmitted = l.DateSubmitted
            }));
        }
        // GET: api/Admin/dashboard
        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboard()
        {
            var totalEmployees =
                await _employeeRepository.GetEmployeeCountAsync();

            var totalLeaveRequests =
                await _leaveRepository.GetTotalLeaveRequestsAsync();

            var pendingLeaves =
                await _leaveRepository
                    .GetLeaveRequestsCountByStatusAsync(LeaveConstants.Pending);

            var approvedLeaves =
                await _leaveRepository
                    .GetLeaveRequestsCountByStatusAsync(LeaveConstants.Approved);

            var rejectedLeaves =
                await _leaveRepository
                    .GetLeaveRequestsCountByStatusAsync(LeaveConstants.Rejected);

            return Ok(new
            {
                totalEmployees,
                totalLeaveRequests,
                pendingLeaves,
                approvedLeaves,
                rejectedLeaves
            });
        }
    }
    
}