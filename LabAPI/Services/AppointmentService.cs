using LabAPI.Constants;
using LabAPI.DTOs;
using LabAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace LabAPI.Services;
public class AppointmentService : IAppointmentService
{
    private readonly MedicalLabsContext _context;

    public AppointmentService(MedicalLabsContext context)
    {
        _context = context;
    }

    public async Task<bool> CreateAppointment(CreateAppointmentRequest request)
    {
        bool isSlotTaken = await _context.Appointments
            .AnyAsync(a => a.OfficeId == request.OfficeId
                && a.VisitDate == request.VisitDate
                && a.VisitTime == request.VisitTime);

        if (isSlotTaken)
        {
            return false;
        }


        await _context.Appointments.AddAsync(new Appointment
        {
            OfficeId = request.OfficeId,
            PatientId = request.PatientId,
            VisitDate = request.VisitDate,
            VisitTime = request.VisitTime,
            Purpose = request.Purpose
        });
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<List<AppointmentResponse>> GetDailyAppointments(int officeId, DateOnly date)
    {
        var appointments = await _context.Appointments
            .Where(a => a.OfficeId == officeId
                && a.VisitDate == date)
            .Select(a => new AppointmentResponse
            {
                Id = a.Id,
                VisitTime = a.VisitTime,
                Status = a.Status,
                Purpose = a.Purpose,
                PatientId = a.PatientId,
                PatientFirstName = a.Patient.FirstName,
                PatientLastName = a.Patient.LastName,
                PatientPhone = a.Patient.Phone
            })
            .ToListAsync();

        return appointments;
    }

    public async Task AdvanceAppointment(int appId)
    {
        var appointment = await _context.Appointments.FindAsync(appId);
        
        if (appointment != null && GetNextStatus(appointment.Status) != null)
        {
            appointment.Status = GetNextStatus(appointment.Status)!;
            await _context.SaveChangesAsync();
        }
    }

    public async Task CancelAppointment(int appId)
    {
        await _context.Appointments
            .Where(a => a.Id == appId)
            .ExecuteUpdateAsync(a => a
                .SetProperty(x => x.Status, AppointmentStatuses.Cancelled));
    }

    private string? GetNextStatus(string status)
    {
        switch (status)
        {
            case AppointmentStatuses.Pending:
                return AppointmentStatuses.Arrived;
            case AppointmentStatuses.Arrived:
                return AppointmentStatuses.Completed;
            default:
                return null;
        }
    }
}
