using LabAPI.DTOs;

namespace LabAPI.Services;
public interface IAppointmentService
{
    Task<bool> CreateAppointment(CreateAppointmentRequest request);
    Task<List<AppointmentResponse>> GetDailyAppointments(int officeId, DateOnly date);
    Task AdvanceAppointment(int appId);
    Task CancelAppointment(int appId);
}
