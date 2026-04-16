using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LabAPI.Models;

public partial class MedicalLabsContext : IdentityDbContext<AppUser, IdentityRole<int>, int>
{
    public MedicalLabsContext()
    {
    }

    public MedicalLabsContext(DbContextOptions<MedicalLabsContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Analysis> Analyses { get; set; }

    public virtual DbSet<Appointment> Appointments { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<EmployeeSchedule> EmployeeSchedules { get; set; }

    public virtual DbSet<EmployeeShift> EmployeeShifts { get; set; }

    public virtual DbSet<LabOrder> LabOrders { get; set; }

    public virtual DbSet<Office> Offices { get; set; }

    public virtual DbSet<OfficeSchedule> OfficeSchedules { get; set; }

    public virtual DbSet<OrderAnalysis> OrderAnalyses { get; set; }

    public virtual DbSet<Parameter> Parameters { get; set; }

    public virtual DbSet<ParameterNorm> ParameterNorms { get; set; }

    public virtual DbSet<ParameterResult> ParameterResults { get; set; }

    public virtual DbSet<Patient> Patients { get; set; }

    public virtual DbSet<Result> Results { get; set; }

    public virtual DbSet<Sample> Samples { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Analysis>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("analysis_pkey");

            entity.ToTable("analysis");

            entity.HasIndex(e => e.Name, "analysis_name_unique").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ExpiryDays).HasColumnName("expiry_days");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("price");
            entity.Property(e => e.SampleType)
                .HasMaxLength(50)
                .HasColumnName("sample_type");
        });

        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("appointment_pkey");

            entity.ToTable("appointment");

            entity.HasIndex(e => new { e.OfficeId, e.VisitDate }, "idx_appointment_office_date").IsDescending(false, true);

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.OfficeId).HasColumnName("office_id");
            entity.Property(e => e.PatientId).HasColumnName("patient_id");
            entity.Property(e => e.Purpose)
                .HasMaxLength(20)
                .HasColumnName("purpose");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("pending")
                .HasColumnName("status");
            entity.Property(e => e.VisitDate).HasColumnName("visit_date");
            entity.Property(e => e.VisitTime)
                .HasPrecision(0)
                .HasColumnName("visit_time");

            entity.HasOne(d => d.Office).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.OfficeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("appointment_office_fkey");

            entity.HasOne(d => d.Patient).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.PatientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("appointment_patient_fkey");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("employee_pkey");

            entity.ToTable("employee");

            entity.HasIndex(e => e.Email, "employee_email_unique").IsUnique();

            entity.HasIndex(e => e.Phone, "employee_phone_unique").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .HasColumnName("first_name");
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .HasColumnName("last_name");
            entity.Property(e => e.MiddleName)
                .HasMaxLength(50)
                .HasColumnName("middle_name");
            entity.Property(e => e.Phone)
                .HasMaxLength(15)
                .HasColumnName("phone");
        });

        modelBuilder.Entity<EmployeeSchedule>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("est_pkey");

            entity.ToTable("employee_schedule");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DayOfWeek).HasColumnName("day_of_week");
            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.EndTime)
                .HasPrecision(0)
                .HasColumnName("end_time");
            entity.Property(e => e.OfficeId).HasColumnName("office_id");
            entity.Property(e => e.StartTime)
                .HasPrecision(0)
                .HasColumnName("start_time");

            entity.HasOne(d => d.Employee).WithMany(p => p.EmployeeSchedules)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("est_employee_fkey");

            entity.HasOne(d => d.Office).WithMany(p => p.EmployeeSchedules)
                .HasForeignKey(d => d.OfficeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("est_office_fkey");
        });

        modelBuilder.Entity<EmployeeShift>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("es_pkey");

            entity.ToTable("employee_shift");

            entity.HasIndex(e => new { e.EmployeeId, e.ShiftDate }, "es_employee_date_unique").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.EndTime)
                .HasPrecision(0)
                .HasColumnName("end_time");
            entity.Property(e => e.OfficeId).HasColumnName("office_id");
            entity.Property(e => e.ShiftDate).HasColumnName("shift_date");
            entity.Property(e => e.ShiftType)
                .HasMaxLength(20)
                .HasDefaultValue("work")
                .HasColumnName("shift_type");
            entity.Property(e => e.StartTime)
                .HasPrecision(0)
                .HasColumnName("start_time");

            entity.HasOne(d => d.Employee).WithMany(p => p.EmployeeShifts)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("es_employee_fkey");

            entity.HasOne(d => d.Office).WithMany(p => p.EmployeeShifts)
                .HasForeignKey(d => d.OfficeId)
                .HasConstraintName("es_office_fkey");
        });

        modelBuilder.Entity<LabOrder>(entity =>
        {
            entity.HasKey(e => e.Number).HasName("lab_order_pkey");

            entity.ToTable("lab_order");

            entity.Property(e => e.Number).HasColumnName("number");
            entity.Property(e => e.CreatedDate)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("created_date");
            entity.Property(e => e.PatientId).HasColumnName("patient_id");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("unpaid")
                .HasColumnName("status");
            entity.Property(e => e.TotalPrice).HasColumnName("total_price");

            entity.HasOne(d => d.Patient).WithMany(p => p.LabOrders)
                .HasForeignKey(d => d.PatientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("lab_order_patient_fkey");

            entity.Property(e => e.OfficeId).HasColumnName("office_id");

            entity.HasOne(d => d.Office).WithMany(p => p.LabOrders)
                .HasForeignKey(d => d.OfficeId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("lab_order_office_fkey");
        });

        modelBuilder.Entity<Office>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("office_pkey");

            entity.ToTable("office");

            entity.HasIndex(e => new { e.Number, e.City }, "office_number_city_unique").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Address)
                .HasMaxLength(200)
                .HasColumnName("address");
            entity.Property(e => e.City)
                .HasMaxLength(50)
                .HasColumnName("city");
            entity.Property(e => e.Number).HasColumnName("number");
            entity.Property(e => e.Type)
                .HasMaxLength(20)
                .HasColumnName("type");
        });

        modelBuilder.Entity<OfficeSchedule>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("office_schedule_pkey");

            entity.ToTable("office_schedule");

            entity.HasIndex(e => new { e.OfficeId, e.DayOfWeek }, "office_schedule_unique").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CloseTime)
                .HasPrecision(0)
                .HasColumnName("close_time");
            entity.Property(e => e.DayOfWeek).HasColumnName("day_of_week");
            entity.Property(e => e.OfficeId).HasColumnName("office_id");
            entity.Property(e => e.OpenTime)
                .HasPrecision(0)
                .HasColumnName("open_time");

            entity.HasOne(d => d.Office).WithMany(p => p.OfficeSchedules)
                .HasForeignKey(d => d.OfficeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("office_schedule_office_fkey");
        });

        modelBuilder.Entity<OrderAnalysis>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("order_analysis_pkey");

            entity.ToTable("order_analysis");

            entity.HasIndex(e => new { e.OrderNumber, e.AnalysisId }, "order_analysis_unique").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AnalysisId).HasColumnName("analysis_id");
            entity.Property(e => e.OrderNumber).HasColumnName("order_number");

            entity.HasOne(d => d.Analysis).WithMany(p => p.OrderAnalyses)
                .HasForeignKey(d => d.AnalysisId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("order_analysis_analysis_fkey");

            entity.HasOne(d => d.OrderNumberNavigation).WithMany(p => p.OrderAnalyses)
                .HasForeignKey(d => d.OrderNumber)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("order_analysis_order_fkey");
        });

        modelBuilder.Entity<Parameter>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("parameter_pkey");

            entity.ToTable("parameter");

            entity.HasIndex(e => new { e.ParameterName, e.AnalysisId }, "parameter_name_analysis_unique").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AnalysisId).HasColumnName("analysis_id");
            entity.Property(e => e.ParameterName)
                .HasMaxLength(100)
                .HasColumnName("parameter_name");
            entity.Property(e => e.Unit)
                .HasMaxLength(20)
                .HasColumnName("unit");

            entity.HasOne(d => d.Analysis).WithMany(p => p.Parameters)
                .HasForeignKey(d => d.AnalysisId)
                .HasConstraintName("parameter_analysis_fkey");
        });

        modelBuilder.Entity<ParameterNorm>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("parameter_norm_pkey");

            entity.ToTable("parameter_norm");

            entity.HasIndex(e => new { e.ParameterId, e.AgeMin, e.AgeMax, e.Gender }, "parameter_norm_unique").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AgeMax).HasColumnName("age_max");
            entity.Property(e => e.AgeMin).HasColumnName("age_min");
            entity.Property(e => e.Gender)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("gender");
            entity.Property(e => e.MaxValue)
                .HasColumnType("decimal(10, 3)")
                .HasColumnName("max_value");
            entity.Property(e => e.MinValue)
                .HasColumnType("decimal(10, 3)")
                .HasColumnName("min_value");
            entity.Property(e => e.ParameterId).HasColumnName("parameter_id");

            entity.HasOne(d => d.Parameter).WithMany(p => p.ParameterNorms)
                .HasForeignKey(d => d.ParameterId)
                .HasConstraintName("parameter_norm_fkey");
        });

        modelBuilder.Entity<ParameterResult>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("parameter_result_pkey");

            entity.ToTable("parameter_result");

            entity.HasIndex(e => new { e.ResultId, e.ParameterId }, "parameter_result_result_parameter_unique").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.IsNormal).HasColumnName("is_normal");
            entity.Property(e => e.MeasuredValue)
                .HasColumnType("decimal(10, 3)")
                .HasColumnName("measured_value");
            entity.Property(e => e.ParameterId).HasColumnName("parameter_id");
            entity.Property(e => e.ResultId).HasColumnName("result_id");

            entity.HasOne(d => d.Parameter).WithMany(p => p.ParameterResults)
                .HasForeignKey(d => d.ParameterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("parameter_result_parameter_fkey");

            entity.HasOne(d => d.Result).WithMany(p => p.ParameterResults)
                .HasForeignKey(d => d.ResultId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("parameter_result_result_fkey");
        });

        modelBuilder.Entity<Patient>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("patient_pkey");

            entity.ToTable("patient");

            entity.HasIndex(e => new { e.LastName, e.FirstName, e.MiddleName }, "idx_patient_name_search");

            entity.HasIndex(e => e.Phone, "patient_phone_unique").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BirthDate).HasColumnName("birth_date");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .HasColumnName("first_name");
            entity.Property(e => e.Gender)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("gender");
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .HasColumnName("last_name");
            entity.Property(e => e.MiddleName)
                .HasMaxLength(50)
                .HasColumnName("middle_name");
            entity.Property(e => e.Phone)
                .HasMaxLength(15)
                .HasColumnName("phone");
        });

        modelBuilder.Entity<Result>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("result_pkey");

            entity.ToTable("result");

            entity.HasIndex(e => e.SampleId, "idx_result_sample_lookup");

            entity.HasIndex(e => new { e.SampleId, e.AnalysisId }, "result_sample_analysis_unique").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AnalysisId).HasColumnName("analysis_id");
            entity.Property(e => e.Conclusion).HasColumnName("conclusion");
            entity.Property(e => e.ResultDate)
                .HasPrecision(0)
                .HasColumnName("result_date");
            entity.Property(e => e.SampleId).HasColumnName("sample_id");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("pending")
                .HasColumnName("status");

            entity.HasOne(d => d.Analysis).WithMany(p => p.Results)
                .HasForeignKey(d => d.AnalysisId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("result_analysis_fkey");

            entity.HasOne(d => d.Sample).WithMany(p => p.Results)
                .HasForeignKey(d => d.SampleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("result_sample_fkey");
        });

        modelBuilder.Entity<Sample>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("sample_pkey");

            entity.ToTable("sample", tb =>
                {
                    tb.HasTrigger("sample_set_expiry_date");
                    tb.HasTrigger("sample_status_sync_order");
                });

            entity.HasIndex(e => e.OrderNumber, "idx_sample_order_search");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CollectionDate)
                .HasPrecision(0)
                .HasColumnName("collection_date");
            entity.Property(e => e.ExpiryDate).HasColumnName("expiry_date");
            entity.Property(e => e.OrderNumber).HasColumnName("order_number");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("waiting")
                .HasColumnName("status");

            entity.HasOne(d => d.OrderNumberNavigation).WithMany(p => p.Samples)
                .HasForeignKey(d => d.OrderNumber)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("sample_order_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
