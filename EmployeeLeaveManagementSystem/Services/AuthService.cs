using EmployeeLeaveManagementSystem.DTOs;
using EmployeeLeaveManagementSystem.Models;
using EmployeeLeaveManagementSystem.Repositories;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using EmployeeLeaveManagementSystem.Data;

namespace EmployeeLeaveManagementSystem.Services
{
    public class AuthService : IAuthService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ILeaveBalanceRepository _leaveBalanceRepository;
        private readonly AppDbContext _context;
        private readonly IPasswordHasher<Employee> _passwordHasher;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
     IEmployeeRepository employeeRepository,
     ILeaveBalanceRepository leaveBalanceRepository,
     IPasswordHasher<Employee> passwordHasher,
     IConfiguration configuration,
     ILogger<AuthService> logger)
        {
            _employeeRepository = employeeRepository;
            _leaveBalanceRepository = leaveBalanceRepository;
            _passwordHasher = passwordHasher;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<string> RegisterAsync(RegisterDto dto)
        {
            var existingEmployee =
                await _employeeRepository.GetByEmailAsync(dto.Email);

            if (existingEmployee != null)
            {
                throw new ArgumentException(
                    "Email is already registered.");
            }

            var employee = new Employee
            {
                Name = dto.Name,
                Email = dto.Email,
                PasswordHash = string.Empty,
                Department = dto.Department,
                Designation = dto.Designation,
                Role = "Employee"
            };

            employee.PasswordHash =
                _passwordHasher.HashPassword(
                    employee,
                    dto.Password);

            await _employeeRepository.AddAsync(employee);

            var leaveBalance = new LeaveBalance
            {
                Employee = employee,
                AnnualLeave = 15,
                SickLeave = 10,
                CasualLeave = 7,
                OtherLeave = 2
            };

            await _leaveBalanceRepository.AddAsync(leaveBalance);

            await _employeeRepository.SaveChangesAsync();
            _logger.LogInformation(
                "Employee registered successfully. EmployeeId: {EmployeeId}",
                employee.EmployeeId);

            return "Registration successful.";
        }

        public async Task<string> LoginAsync(LoginDto dto)
        {
            var employee =
                await _employeeRepository.GetByEmailAsync(dto.Email);

            if (employee == null)
            {
                throw new UnauthorizedAccessException(
                    "Invalid email or password.");
            }

            var passwordResult =
                _passwordHasher.VerifyHashedPassword(
                    employee,
                    employee.PasswordHash,
                    dto.Password);

            if (passwordResult ==
                PasswordVerificationResult.Failed)
            {
                throw new UnauthorizedAccessException(
                    "Invalid email or password.");
            }

            var token = GenerateJwtToken(employee);

            _logger.LogInformation(
                "Employee login successful. EmployeeId: {EmployeeId}",
                employee.EmployeeId);

            return token;
        }

        private string GenerateJwtToken(Employee employee)
        {
            var claims = new[]
            {
                new Claim(
                    ClaimTypes.NameIdentifier,
                    employee.EmployeeId.ToString()),

                new Claim(
                    ClaimTypes.Name,
                    employee.Name),

                new Claim(
                    ClaimTypes.Email,
                    employee.Email),

                new Claim(
                    ClaimTypes.Role,
                    employee.Role)
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    _configuration["Jwt:Key"]!));

            var credentials =
                new SigningCredentials(
                    key,
                    SecurityAlgorithms.HmacSha256);

            var expiryMinutes =
                int.Parse(
                    _configuration["Jwt:ExpiryMinutes"]!);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    expiryMinutes),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler()
                .WriteToken(token);
        }
    }
}