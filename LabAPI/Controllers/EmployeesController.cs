using LabAPI.Constants;
using LabAPI.DTOs;
using LabAPI.Models;
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

    [Authorize(Roles = Roles.Admin)]
    [HttpGet("{employeeId}/regular-schedule")]
    public async Task<IActionResult> GetRegularSchedule(int employeeId)
    {
        return Ok(await _employeeService.GetRegularSchedule(employeeId));
    }

    [Authorize(Roles = Roles.Admin)]
    [HttpPost("regular-schedule")]
    public async Task<IActionResult> CreateRegularShift(CreateRegularShiftRequest request)
    {
        await _employeeService.CreateRegularShift(request);

        return Ok();
    }

    [Authorize(Roles = Roles.Admin)]
    [HttpPut("regular-schedule")]
    public async Task<IActionResult> UpdateRegularShift(UpdateRegularShiftRequest request)
    {
        if (!await _employeeService.UpdateRegularShift(request))
        {
            return BadRequest(new { msg = "Такої зміни не існує" });
        }

        return Ok();
    }

    [Authorize(Roles = Roles.Admin)]
    [HttpDelete("regular-schedule/{regularShiftId}")]
    public async Task<IActionResult> DeleteRegularShift(int regularShiftId)
    {
        if (!await _employeeService.DeleteRegularShift(regularShiftId))
        {
            return BadRequest(new { msg = "Такої зміни не існує" });
        }

        return Ok();
    }

    [Authorize(Roles = Roles.Admin)]
    [HttpGet("{employeeId}/shifts")]
    public async Task<IActionResult> GetShifts(int employeeId, int page, int pageSize, bool includePast)
    {
        var (shifts, pageCount) = await _employeeService
            .GetShifts(employeeId, page, pageSize, includePast);
        
        return Ok(new
        {
            Shifts = shifts,
            PageCount = pageCount
        });
    }

    [Authorize(Roles = Roles.Admin)]
    [HttpPost("shifts")]
    public async Task<IActionResult> CreateShift(CreateShiftRequest request)
    {
        if (!await _employeeService.CreateShift(request))
        {
            return BadRequest(new { msg = "Цей виняток перетинається з іншим" });
        }

        return Ok();
    }

    [Authorize(Roles = Roles.Admin)]
    [HttpDelete("shifts/{shiftId}")]
    public async Task<IActionResult> DeleteShift(int shiftId)
    {
        if (!await _employeeService.DeleteShift(shiftId))
        {
            return BadRequest(new { msg = "Такої зміни не існує" });
        }

        return Ok();
    }
}
