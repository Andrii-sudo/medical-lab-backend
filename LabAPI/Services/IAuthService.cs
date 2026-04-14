using LabAPI.DTOs;
using LabAPI.Models;

namespace LabAPI.Services;

public interface IAuthService
{
    Task<AppUser?> LoginEmployee(LoginEmployeeRequest request);
    Task<AppUser?> LoginPatient(LoginPatientRequest request);
    Task<string> GenerateToken(AppUser identityUser);
    Task<string> GetUserRole(AppUser identityUser);
}
