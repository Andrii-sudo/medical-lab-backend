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
}
