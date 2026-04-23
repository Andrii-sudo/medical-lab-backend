using LabAPI.DTOs;
using LabAPI.Models;

namespace LabAPI.Services;

public interface IOfficeService
{
    Task<OfficeResponse?> GetCurrentEmployeeOffice(int employeeId);
    Task<List<OfficeResponse>> GetEmployeeOffices(int employeeId);
    Task<List<string>> GetOfficeCities();
    Task<List<OfficeByCityResponse>> GetOffices(string city, string? officeType = null);
    Task<List<TimeOnly>?> GetAvailableSlots(int officeId, DateOnly date, int slotSpace = 15);
    Task<(TimeOnly OpenTime, TimeOnly CloseTime)?> GetOfficeWorkingHours(int officeId, byte dayOfWeek);
}

