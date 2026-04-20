using LabAPI.DTOs;
using LabAPI.Models;

namespace LabAPI.Services;

public interface IAuthService
{
    Task<AppUser?> LoginEmployee(EmployeeLoginRequest request);
    Task<AppUser?> LoginPatient(PatientLoginRequest request);
    Task<(string, string)> GenerateToken(AppUser identityUser);
}
