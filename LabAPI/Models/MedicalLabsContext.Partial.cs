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

        modelBuilder.Entity<Patient>()
            .ToTable(t => t.HasCheckConstraint("patient_gender_check", "gender IN ('M','F')"));

        modelBuilder.Entity<Office>()
            .ToTable(t => t.HasCheckConstraint("office_type_check", "type IN ('collection', 'analysis', 'mixed')"));

        modelBuilder.Entity<Analysis>()
            .ToTable(t => t.HasCheckConstraint("analysis_price_check", "price >= 0"));

        modelBuilder.Entity<LabOrder>()
            .ToTable(t => 
            t.HasCheckConstraint(
                "lab_order_status_check", 
                "status IN ('unpaid', 'pending', 'in_progress', 'completed', 'cancelled')"));

        modelBuilder.Entity<Sample>()
            .ToTable(t =>
            {
                t.HasCheckConstraint(
                    "sample_status_check",
                    "status IN ('waiting', 'collected', 'analyzed', 'expired')");
                t.HasCheckConstraint(
                    "sample_collection_expiry_date_check",
                    "expiry_date IS NULL OR (collection_date IS NOT NULL AND collection_date < expiry_date)");
            });

        modelBuilder.Entity<ParameterNorm>()
            .ToTable(t =>
            { 
                t.HasCheckConstraint("parameter_norm_age_min_check", "age_min >= 0");
                t.HasCheckConstraint("parameter_norm_age_max_check", "age_max >= age_min");
                t.HasCheckConstraint("parameter_norm_gender_check", "gender IN ('M','F','A')");
                t.HasCheckConstraint("parameter_norm_val_null_check", "max_value IS NOT NULL OR min_value IS NOT NULL");
                t.HasCheckConstraint(
                    "parameter_norm_max_val_check", 
                    "max_value IS NULL OR min_value IS NULL OR max_value >= min_value");
            });
       
        modelBuilder.Entity<Result>()
            .ToTable(t => t.HasCheckConstraint("result_status_check", "status IN ('pending', 'normal', 'abnormal')"));

        modelBuilder.Entity<EmployeeSchedule>()
            .ToTable(t =>
            { 
                t.HasCheckConstraint("est_day_check", "day_of_week BETWEEN 0 AND 6");
                t.HasCheckConstraint("est_time_check", "end_time > start_time");
            });

        modelBuilder.Entity<EmployeeShift>()
            .ToTable(t =>
            {
                t.HasCheckConstraint("es_type_check", "shift_type IN ('work', 'day_off', 'sick_leave', 'vacation')");
                t.HasCheckConstraint(
                    "es_time_check",
                    "(shift_type = 'work' AND start_time IS NOT NULL AND end_time IS NOT NULL AND end_time > start_time) " +
                    "OR (shift_type <> 'work' AND start_time IS NULL AND end_time IS NULL)");
                t.HasCheckConstraint(
                    "es_office_check",
                    "(shift_type = 'work'  AND office_id IS NOT NULL) OR (shift_type <> 'work' AND office_id IS NULL)");
            });

        modelBuilder.Entity<Appointment>()
            .ToTable(t =>
            { 
                t.HasCheckConstraint("appointment_purpose_check", "purpose IN ('first_visit', 'sample', 'results')");
                t.HasCheckConstraint("appointment_status_check", "status IN ('pending', 'arrived', 'completed', 'cancelled')");
            });

        modelBuilder.Entity<OfficeSchedule>()
            .ToTable(t =>
            {
                t.HasCheckConstraint("office_schedule_day_check", "day_of_week BETWEEN 0 AND 6");
                t.HasCheckConstraint("office_schedule_time_check", "close_time > open_time");
            });
    }
}
