using LabAPI.Constants;
using LabAPI.DTOs;
using LabAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace LabAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _employeeService;

    public EmployeesController(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    [Authorize(Roles = Roles.Admin)]
    [HttpGet]
    public async Task<IActionResult> GetEmployees(int page, int pageSize, string? searchTerm)
    {
        var (employees, pageCount) = await _employeeService.GetEmployees(page, pageSize, searchTerm);

        return Ok(new
        {
            Employees = employees,
            PageCount = pageCount
        });
    }

    [Authorize(Roles = Roles.Admin)]
    [HttpPost]
    public async Task<IActionResult> CreateEmployee(CreateEmployeeRequest request)
    {
        var msg = await _employeeService.CreateEmployee(request);
        if (msg != null)
        {
            return BadRequest(new { msg = msg });
        }

        return Created();
    }

    [Authorize(Roles = Roles.Admin)]
    [HttpPut]
    public async Task<IActionResult> UpdateEmployee(UpdateEmployeeRequest request)
    {
        var msg = await _employeeService.UpdateEmployee(request);
        if (msg != null)
        {
            return BadRequest(new { msg = msg });
        }

        return Ok();
    }

    [Authorize(Roles = Roles.Admin)]
    [HttpDelete("{employeeId}")]
    public async Task<IActionResult> DeleteEmployee(int employeeId)
    {
        if (!await _employeeService.DeleteEmployee(employeeId))
        {
            return BadRequest(new { msg = "Такого працівника не існує" });
        }

        return Ok();
    }
}
