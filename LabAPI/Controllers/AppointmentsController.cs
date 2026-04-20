using LabAPI.Constants;
using LabAPI.DTOs;
using LabAPI.Models;
using LabAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LabAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AppointmentsController : ControllerBase
{
    private readonly IAppointmentService _appointmentService;

    public AppointmentsController(IAppointmentService appointmentService)
    {
        _appointmentService = appointmentService;
    }

    [Authorize(Roles = $"{Roles.Admin},{Roles.Employee}")]
    [HttpPost]
    public async Task<IActionResult> CreateAppointment(CreateAppointmentRequest request)
    {
        if (!await _appointmentService.CreateAppointment(request))
        {
            return BadRequest(new { msg = "Цей час для вже занятий" });
        }

        return Created();
    }

    [Authorize(Roles = $"{Roles.Admin},{Roles.Employee}")]
    [HttpGet]
    public async Task<IActionResult> GetDailyAppointments(int officeId, DateOnly date)
    {
        return Ok(await _appointmentService.GetDailyAppointments(officeId, date));
    }

    [Authorize(Roles = $"{Roles.Admin},{Roles.Employee}")]
    [HttpPut("{appId}/advance")]
    public async Task<IActionResult> AdvanceAppointment(int appId)
    {
        await _appointmentService.AdvanceAppointment(appId);

        return Ok();
    }

    [Authorize(Roles = $"{Roles.Admin},{Roles.Employee}")]
    [HttpPut("{appId}/cancel")]
    public async Task<IActionResult> CancelAppointment(int appId)
    {
        await _appointmentService.CancelAppointment(appId);

        return Ok();
    }
}
