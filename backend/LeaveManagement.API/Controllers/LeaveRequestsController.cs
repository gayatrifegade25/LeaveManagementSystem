using System.Security.Claims;
using LeaveManagement.API.Data;
using LeaveManagement.API.DTOs;
using LeaveManagement.API.Models;
using LeaveManagement.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LeaveRequestsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILeaveRequestService _leaveRequestService;

    public LeaveRequestsController(ApplicationDbContext context, ILeaveRequestService leaveRequestService)
    {
        _context = context;
        _leaveRequestService = leaveRequestService;
    }

    private int CurrentEmployeeId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    private bool IsManager => User.IsInRole("Manager");

    // GET: api/leaverequests
    // Employees see their own requests; managers see everyone's.
    [HttpGet]
    public async Task<ActionResult<IEnumerable<LeaveRequestResponseDto>>> GetAll()
    {
        var query = _context.LeaveRequests.Include(lr => lr.Employee).AsQueryable();

        if (!IsManager)
            query = query.Where(lr => lr.EmployeeId == CurrentEmployeeId);

        var results = await query
            .OrderByDescending(lr => lr.CreatedAt)
            .Select(lr => new LeaveRequestResponseDto
            {
                Id = lr.Id,
                EmployeeName = lr.Employee!.FullName,
                StartDate = lr.StartDate,
                EndDate = lr.EndDate,
                TotalDays = lr.TotalDays,
                Reason = lr.Reason,
                Status = lr.Status.ToString(),
                ManagerComment = lr.ManagerComment
            })
            .ToListAsync();

        return Ok(results);
    }

    // POST: api/leaverequests
    [HttpPost]
    public async Task<IActionResult> Create(CreateLeaveRequestDto dto)
    {
        if (!_leaveRequestService.IsValidRange(dto.StartDate, dto.EndDate))
            return BadRequest("End date must be on or after the start date.");

        var employee = await _context.Employees.FindAsync(CurrentEmployeeId);
        if (employee is null) return Unauthorized();

        var requestedDays = (dto.EndDate - dto.StartDate).Days + 1;
        if (!_leaveRequestService.HasSufficientBalance(employee, requestedDays))
            return BadRequest("Insufficient leave balance.");

        var leaveRequest = new LeaveRequest
        {
            EmployeeId = employee.Id,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Reason = dto.Reason
        };

        _context.LeaveRequests.Add(leaveRequest);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetAll), new { id = leaveRequest.Id }, leaveRequest);
    }

    // PUT: api/leaverequests/5/review  (manager only)
    [HttpPut("{id}/review")]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> Review(int id, ReviewLeaveRequestDto dto)
    {
        var leaveRequest = await _context.LeaveRequests
            .Include(lr => lr.Employee)
            .FirstOrDefaultAsync(lr => lr.Id == id);

        if (leaveRequest is null) return NotFound();
        if (leaveRequest.Status != LeaveStatus.Pending)
            return BadRequest("This request has already been reviewed.");

        leaveRequest.Status = dto.Approve ? LeaveStatus.Approved : LeaveStatus.Rejected;
        leaveRequest.ManagerComment = dto.Comment;

        if (dto.Approve && leaveRequest.Employee is not null)
            leaveRequest.Employee.LeaveBalance -= leaveRequest.TotalDays;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: api/leaverequests/5  (owner can cancel while still pending)
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var leaveRequest = await _context.LeaveRequests.FindAsync(id);
        if (leaveRequest is null) return NotFound();
        if (leaveRequest.EmployeeId != CurrentEmployeeId) return Forbid();
        if (leaveRequest.Status != LeaveStatus.Pending)
            return BadRequest("Only pending requests can be cancelled.");

        _context.LeaveRequests.Remove(leaveRequest);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
