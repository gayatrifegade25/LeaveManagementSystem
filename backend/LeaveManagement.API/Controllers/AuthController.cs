using LeaveManagement.API.Data;
using LeaveManagement.API.DTOs;
using LeaveManagement.API.Models;
using LeaveManagement.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ITokenService _tokenService;

    public AuthController(ApplicationDbContext context, ITokenService tokenService)
    {
        _context = context;
        _tokenService = tokenService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        if (await _context.Employees.AnyAsync(e => e.Email == dto.Email))
            return BadRequest("An account with this email already exists.");

        var employee = new Employee
        {
            FullName = dto.FullName,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = dto.Role
        };

        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();

        return Ok(new { token = _tokenService.CreateToken(employee) });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var employee = await _context.Employees.SingleOrDefaultAsync(e => e.Email == dto.Email);
        if (employee is null || !BCrypt.Net.BCrypt.Verify(dto.Password, employee.PasswordHash))
            return Unauthorized("Invalid email or password.");

        return Ok(new { token = _tokenService.CreateToken(employee) });
    }
}
