using LeaveManagement.API.Models;
using LeaveManagement.API.Services;
using Xunit;

namespace LeaveManagement.Tests;

public class LeaveRequestServiceTests
{
    private readonly LeaveRequestService _sut = new();

    [Fact]
    public void IsValidRange_ReturnsTrue_WhenEndDateIsAfterStartDate()
    {
        var start = new DateTime(2026, 9, 1);
        var end = new DateTime(2026, 9, 5);

        Assert.True(_sut.IsValidRange(start, end));
    }

    [Fact]
    public void IsValidRange_ReturnsFalse_WhenEndDateIsBeforeStartDate()
    {
        var start = new DateTime(2026, 9, 5);
        var end = new DateTime(2026, 9, 1);

        Assert.False(_sut.IsValidRange(start, end));
    }

    [Theory]
    [InlineData(20, 5, true)]
    [InlineData(3, 5, false)]
    [InlineData(5, 5, true)]
    public void HasSufficientBalance_ChecksCorrectly(int balance, int requestedDays, bool expected)
    {
        var employee = new Employee { LeaveBalance = balance };

        Assert.Equal(expected, _sut.HasSufficientBalance(employee, requestedDays));
    }
}
