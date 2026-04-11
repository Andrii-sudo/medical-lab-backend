using LabAPI.DTOs;
using LabAPI.Models;

namespace LabAPI.Services
{
    public interface IAuthService
    {
        Task<AppUser?> LoginEmployee(LoginEmployeeRequest request);
        Task<AppUser?> LoginPatient(LoginPatientRequest request);
        Task<string> GenerateTokenString(AppUser identityUser);
    }
}