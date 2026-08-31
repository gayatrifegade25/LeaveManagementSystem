using LeaveManagement.API.Models;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagement.API.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<LeaveRequest> LeaveRequests => Set<LeaveRequest>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Employee>()
            .HasIndex(e => e.Email)
            .IsUnique();

        modelBuilder.Entity<LeaveRequest>()
            .HasOne(lr => lr.Employee)
            .WithMany(e => e.LeaveRequests)
            .HasForeignKey(lr => lr.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);

        base.OnModelCreating(modelBuilder);
    }
}
