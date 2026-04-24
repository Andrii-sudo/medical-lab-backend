using Azure.Core;
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
    Task<(List<OfficeResponse>, int)> GetOfficesPage(int page, int pageSize, string? city, string? officeType);

    Task CreateOffice(CreateOfficeRequest request);

    Task<List<OfficeScheduleDto>> GetOfficeSchedule(int officeId);
    Task UpdateOfficeSchedule(int officeId, List<OfficeScheduleDto> schedule);
}

