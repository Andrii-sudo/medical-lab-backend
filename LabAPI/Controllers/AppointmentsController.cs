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
    private readonly IAppointmentService _appoinmentService;

    public AppointmentsController(IAppointmentService appointmentService)
    {
        _appoinmentService = appointmentService;
    }

    [Authorize(Roles = $"{Roles.Admin},{Roles.Employee}")]
    [HttpPost]
    public async Task<IActionResult> CreateAppointment(CreateAppointmentRequest request)
    {
        await _appoinmentService.Create(request);

        return Created();
    }
}
