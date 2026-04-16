using LabAPI.Constants;
using LabAPI.DTOs;
using LabAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LabAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [Authorize(Roles = Roles.Employee)]
    [HttpGet("EmployeeStats")]
    public async Task<IActionResult> GetEmployeeStats(int officeId)
    {
        var employeeStats = new EmployeeStatsResponse
        {
            PlannedVisitors = await _dashboardService.GetPlannedVisitors(officeId),
            PendingSamples = await _dashboardService.GetPendingSamples(officeId),
            ProcessingSamples = await _dashboardService.GetProcessingSamples(officeId),
            CompletedResults = await _dashboardService.GetCompletedResults(officeId)
        };


        return Ok(employeeStats);
    }

    [Authorize(Roles = Roles.Employee)]
    [HttpGet("EmployeeShifts")]
    public async Task<IActionResult> GetEmployeeShifts(int employeeId)
    {
        return Ok(await _dashboardService.GetEmployeeSchedule(employeeId));
    }

    [Authorize(Roles = Roles.Employee)]
    [HttpGet("EmployeeSamples")]
    public async Task<IActionResult> GetEmployeeSamples(int officeId)
    {
        return Ok(await _dashboardService.GetEmployeeSamples(officeId));
    }
}
