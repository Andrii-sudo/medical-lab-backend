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
}
