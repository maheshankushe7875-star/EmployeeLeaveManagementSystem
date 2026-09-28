using EmployeeLeaveManagementSystem.Data;
using EmployeeLeaveManagementSystem.DTOs;
using EmployeeLeaveManagementSystem.Models;
using EmployeeLeaveManagementSystem.Repositories;
using EmployeeLeaveManagementSystem.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace EmployeeLeaveManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthController> _logger;
        private readonly IPasswordHasher<Employee> _passwordHasher;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IAuthService _authService;


        public AuthController(AppDbContext context,
             IAuthService authService,
             IEmployeeRepository employeeRepository,
            IConfiguration configuration,
            ILogger<AuthController> logger,
            IPasswordHasher<Employee> passwordHasher)
        {
            _context = context;
            _authService = authService;
            _employeeRepository = employeeRepository;
            _configuration = configuration;
            _logger = logger;
            _passwordHasher = passwordHasher;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var message = await _authService.RegisterAsync(dto);

            return Ok(new
            {
                message
            });
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var token = await _authService.LoginAsync(dto);

            return Ok(new
            {
                message = "Login successful.",
                token
            });
        }





    }
}
