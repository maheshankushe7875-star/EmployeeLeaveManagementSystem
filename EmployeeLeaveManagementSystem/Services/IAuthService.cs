using EmployeeLeaveManagementSystem.DTOs;

namespace EmployeeLeaveManagementSystem.Services
{
    public interface IAuthService
    {
        Task<string> RegisterAsync(RegisterDto dto);

        Task<string> LoginAsync(LoginDto dto);
    }
}