using LabAPI.DTOs;

namespace LabAPI.Services;

public interface IOfficeService
{
    Task<OfficeResponse?> GetCurrentEmployeeOffice(int employeeId);
    Task<List<OfficeResponse>> GetEmployeeOffices(int employeeId);
}

