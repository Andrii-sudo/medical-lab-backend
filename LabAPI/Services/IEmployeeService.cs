using Azure.Core;
using LabAPI.DTOs;
using LabAPI.Models;

namespace LabAPI.Services;

public interface IEmployeeService
{
    IQueryable<Employee> GetEmployeesBySearchTerm(string searchTerm);
    Task<(List<EmployeeResponse>, int)> GetEmployees(int page, int pageSize, string? searchTerm);
    Task<string?> CreateEmployee(CreateEmployeeRequest request);
    Task<string?> UpdateEmployee(UpdateEmployeeRequest request);
    Task<bool> DeleteEmployee(int employeeId);

    Task<List<RegularShiftResponse?>> GetRegularSchedule(int employeeId);
    Task<bool> DeleteRegularShift(int regularShiftId);
    Task CreateRegularShift(CreateRegularShiftRequest request);
    Task<bool> UpdateRegularShift(UpdateRegularShiftRequest request);

    Task<(List<ShiftResponse>, int)> GetShifts(int employeeId, int page, int pageSize, bool includePast);
    Task<bool> CreateShift(CreateShiftRequest request);
    Task<bool> DeleteShift(int shiftId);
}
