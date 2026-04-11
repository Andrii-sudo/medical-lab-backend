using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LabAPI.Models;
public partial class MedicalLabsContext
{
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AppUser>(entity =>
        {
            entity.HasOne(u => u.Employee).WithOne()
                .HasForeignKey<AppUser>(u => u.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(u => u.Patient).WithOne()
                .HasForeignKey<AppUser>(u => u.PatientId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
