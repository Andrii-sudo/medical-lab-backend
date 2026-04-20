using LabAPI.DTOs;

namespace LabAPI.Services;
public interface IAppointmentService
{
    Task Create(CreateAppointmentRequest request);
}
