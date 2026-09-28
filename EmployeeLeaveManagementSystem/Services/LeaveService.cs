using EmployeeLeaveManagementSystem.Data;
using EmployeeLeaveManagementSystem.DTOs;
using EmployeeLeaveManagementSystem.Models;
using EmployeeLeaveManagementSystem.Repositories;
using Microsoft.EntityFrameworkCore;
using EmployeeLeaveManagementSystem.Constants;

namespace EmployeeLeaveManagementSystem.Services
{
    public class LeaveService : ILeaveService
    {
        private readonly AppDbContext _context;
        private readonly ILeaveRepository _leaveRepository;
        private readonly ILeaveBalanceRepository _leaveBalanceRepository;
        private readonly ILogger<LeaveService> _logger;
        private readonly INotificationRepository _notificationRepository;

        public LeaveService(
            AppDbContext context,
            ILeaveRepository leaveRepository,
            ILeaveBalanceRepository leaveBalanceRepository,
            INotificationRepository notificationRepository,
            ILogger<LeaveService> logger)
        {
            _context = context;
            _leaveRepository = leaveRepository;
            _leaveBalanceRepository = leaveBalanceRepository;
            _notificationRepository = notificationRepository;
            _logger = logger;
        }

        // =========================
        // APPLY LEAVE
        // =========================
        public async Task<LeaveRequest> ApplyLeaveAsync(
            int employeeId,
            ApplyLeaveDto dto)
        {
            if (dto.StartDate > dto.EndDate)
            {
                throw new ArgumentException(
                    "Start date cannot be after end date.");
            }

            if (dto.StartDate.Date < DateTime.UtcNow.Date)
            {
                throw new ArgumentException(
                    "Leave cannot be applied for a past date.");
            }
            var validLeaveTypes = new[]
{
    LeaveConstants.Annual,
    LeaveConstants.Sick,
    LeaveConstants.Casual,
    LeaveConstants.Other
};

            if (!validLeaveTypes.Contains(
                    dto.LeaveType,
                    StringComparer.OrdinalIgnoreCase))
            {
                throw new ArgumentException(
                    "Invalid leave type. Allowed values are Annual, Sick, Casual, Other.");
            }
            var leaveType = validLeaveTypes
    .First(x => x.Equals(
        dto.LeaveType,
        StringComparison.OrdinalIgnoreCase));

             

            

            var totalDays =
                (dto.EndDate.Date - dto.StartDate.Date).Days + 1;

            var overlappingLeave =
                await _leaveRepository.HasOverlappingLeaveAsync(
                    employeeId,
                    dto.StartDate,
                    dto.EndDate);

            if (overlappingLeave)
            {
                throw new ArgumentException(
                    "You already have a leave request for these dates.");
            }

            var balance =
                await _leaveBalanceRepository.GetByEmployeeIdAsync(
                    employeeId);

            if (balance == null)
            {
                throw new InvalidOperationException(
                    "Leave balance not found.");
            }

            int availableLeaves;

            switch (leaveType)
            {
                case "Annual":
                    availableLeaves = balance.AnnualLeave;
                    break;

                case "Sick":
                    availableLeaves = balance.SickLeave;
                    break;

                case "Casual":
                    availableLeaves = balance.CasualLeave;
                    break;

                case "Other":
                    availableLeaves = balance.OtherLeave;
                    break;

                default:
                    throw new ArgumentException(
                        "Invalid leave type.");
            }

            if (totalDays > availableLeaves)
            {
                throw new ArgumentException(
                    "Insufficient leave balance.");
            }

            var leaveRequest = new LeaveRequest
            {
                EmployeeId = employeeId,
                LeaveType = leaveType,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Reason = dto.Reason,
                Status = "Pending",
                DateSubmitted = DateTime.UtcNow
            };

            await _leaveRepository.AddAsync(leaveRequest);

            await _leaveRepository.SaveChangesAsync();

            _logger.LogInformation(
                "Leave request {LeaveRequestId} submitted by employee {EmployeeId}.",
                leaveRequest.LeaveRequestId,
                employeeId);

            return leaveRequest;
        }


        // =========================
        // APPROVE LEAVE
        // =========================
        public async Task<LeaveRequest> ApproveLeaveAsync(
    int leaveRequestId,
    string remarks)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                var leaveRequest =
                    await _leaveRepository.GetByIdAsync(
                        leaveRequestId);

                if (leaveRequest == null)
                {
                    throw new KeyNotFoundException(
                        "Leave request not found.");
                }

                if (!string.Equals(
        leaveRequest.Status?.Trim(),
        "Pending",
        StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException(
                        "Only pending leave requests can be approved.");
                }

                var balance =
                    await _leaveBalanceRepository
                        .GetByEmployeeIdAsync(
                            leaveRequest.EmployeeId);

                if (balance == null)
                {
                    throw new InvalidOperationException(
                        "Leave balance not found.");
                }

                var totalDays =
                    (leaveRequest.EndDate.Date -
                     leaveRequest.StartDate.Date).Days + 1;

                switch (leaveRequest.LeaveType)
                {
                    case LeaveConstants.Annual:

                        if (balance.AnnualLeave < totalDays)
                        {
                            throw new InvalidOperationException(
                                "Insufficient annual leave balance.");
                        }

                        balance.AnnualLeave -= totalDays;
                        break;

                    case LeaveConstants.Sick:

                        if (balance.SickLeave < totalDays)
                        {
                            throw new InvalidOperationException(
                                "Insufficient sick leave balance.");
                        }

                        balance.SickLeave -= totalDays;
                        break;

                    case LeaveConstants.Casual:

                        if (balance.CasualLeave < totalDays)
                        {
                            throw new InvalidOperationException(
                                "Insufficient casual leave balance.");
                        }

                        balance.CasualLeave -= totalDays;
                        break;

                    case LeaveConstants.Other:

                        if (balance.OtherLeave < totalDays)
                        {
                            throw new InvalidOperationException(
                                "Insufficient other leave balance.");
                        }

                        balance.OtherLeave -= totalDays;
                        break;

                    default:

                        throw new InvalidOperationException(
                            "Invalid leave type.");
                }

                leaveRequest.Status = LeaveConstants.Approved;
                leaveRequest.AdminRemark = remarks;

                await _leaveRepository.SaveChangesAsync();

                await _notificationRepository.AddAsync(
                    new Notification
                    {
                        EmployeeId = leaveRequest.EmployeeId,
                        Message =
                            $"Your {leaveRequest.LeaveType} leave request " +
                            $"from {leaveRequest.StartDate:dd-MM-yyyy} " +
                            $"to {leaveRequest.EndDate:dd-MM-yyyy} " +
                            $"has been approved.",
                        IsRead = false,
                        CreatedAt = DateTime.UtcNow
                    });

                await _notificationRepository.SaveChangesAsync();

                await transaction.CommitAsync();

                _logger.LogInformation(
                    "Leave request {LeaveRequestId} approved for employee {EmployeeId}.",
                    leaveRequest.LeaveRequestId,
                    leaveRequest.EmployeeId);

                return leaveRequest;
            }
            catch
            {
                await transaction.RollbackAsync();

                throw;
            }
        }


        // =========================
        // REJECT LEAVE
        // =========================
        public async Task<LeaveRequest> RejectLeaveAsync(
    int leaveRequestId,
    string remarks)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                var leaveRequest =
                    await _leaveRepository.GetByIdAsync(
                        leaveRequestId);

                if (leaveRequest == null)
                {
                    throw new KeyNotFoundException(
                        "Leave request not found.");
                }

                if (leaveRequest.Status != LeaveConstants.Pending)
                {
                    throw new InvalidOperationException(
                        "Only pending leave requests can be rejected.");
                }

                leaveRequest.Status = LeaveConstants.Rejected;
                leaveRequest.AdminRemark = remarks;

                await _leaveRepository.SaveChangesAsync();

                await _notificationRepository.AddAsync(
                    new Notification
                    {
                        EmployeeId = leaveRequest.EmployeeId,
                        Message =
                            $"Your {leaveRequest.LeaveType} leave request " +
                            $"from {leaveRequest.StartDate:dd-MM-yyyy} " +
                            $"to {leaveRequest.EndDate:dd-MM-yyyy} " +
                            $"has been rejected.",
                        IsRead = false,
                        CreatedAt = DateTime.UtcNow
                    });

                await _notificationRepository.SaveChangesAsync();

                await transaction.CommitAsync();

                _logger.LogInformation(
                    "Leave request {LeaveRequestId} rejected for employee {EmployeeId}.",
                    leaveRequest.LeaveRequestId,
                    leaveRequest.EmployeeId);

                return leaveRequest;
            }
            catch
            {
                await transaction.RollbackAsync();

                throw;
            }
        }
    }
}