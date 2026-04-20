using LabAPI.DTOs;

namespace LabAPI.Services;
public interface IAppointmentService
{
    Task<bool> CreateAppointment(CreateAppointmentRequest request);
}
