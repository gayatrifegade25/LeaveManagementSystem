using LeaveManagement.API.Models;

namespace LeaveManagement.API.Services;

public interface ILeaveRequestService
{
    bool IsValidRange(DateTime start, DateTime end);
    bool HasSufficientBalance(Employee employee, int requestedDays);
}

public class LeaveRequestService : ILeaveRequestService
{
    public bool IsValidRange(DateTime start, DateTime end)
    {
        return end >= start;
    }

    public bool HasSufficientBalance(Employee employee, int requestedDays)
    {
        return employee.LeaveBalance >= requestedDays;
    }
}
