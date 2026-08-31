namespace LeaveManagement.API.Models;

public class LeaveRequest
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public Employee? Employee { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Reason { get; set; } = string.Empty;
    public LeaveStatus Status { get; set; } = LeaveStatus.Pending;
    public string? ManagerComment { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int TotalDays => (EndDate - StartDate).Days + 1;
}
