using LabAPI.DTOs;
using LabAPI.Models;

namespace LabAPI.Services;

public interface IDashboardService
{
    Task<int> GetPlannedVisitors(int officeId);
    Task<int> GetPendingSamples(int officeId);
    Task<int> GetProcessingSamples(int officeId);
    Task<int> GetCompletedResults(int officeId);
    Task<List<EmployeeShiftResponse>> GetEmployeeSchedule(int employeeId, int dayRange = 7);
    Task<List<EmployeeSampleResponse>> GetEmployeeSamples(int officeId, int count = 7);
}
