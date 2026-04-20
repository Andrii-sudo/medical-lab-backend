using LabAPI.DTOs;
using LabAPI.Models;

namespace LabAPI.Services;
public class AppointmentService : IAppointmentService
{
    private readonly MedicalLabsContext _context;

    public AppointmentService(MedicalLabsContext context)
    {
        _context = context;
    }

    public async Task Create(CreateAppointmentRequest request)
    {
        await _context.Appointments.AddAsync(new Appointment
        {
            OfficeId = request.OfficeId,
            PatientId = request.PatientId,
            VisitDate = request.VisitDate,
            VisitTime = request.VisitTime,
            Purpose = request.purpose
        });
        await _context.SaveChangesAsync();
    }
}
