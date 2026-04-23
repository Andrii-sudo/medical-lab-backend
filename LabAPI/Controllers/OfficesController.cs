using LabAPI.Constants;
using LabAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LabAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OfficesController : ControllerBase
{
    private readonly IOfficeService _officeService;

    public OfficesController(IOfficeService officeService)
    {
        _officeService = officeService;
    }

    [Authorize(Roles = Roles.Employee)]
    [HttpGet("employee/current")]
    public async Task<IActionResult> GetCurrentEmployeeOffice(int employeeId)
    {
        var officeResponse = await _officeService.GetCurrentEmployeeOffice(employeeId);

        if (officeResponse == null)
        {
            return NoContent();
        }
        
        return Ok(officeResponse);    
    }

    [Authorize(Roles = Roles.Employee)]
    [HttpGet("employee")]
    public async Task<IActionResult> GetEmployeeOffices(int employeeId)
    {
        var officesResponse = await _officeService.GetEmployeeOffices(employeeId);

        return Ok(officesResponse);
    }

    [HttpGet("cities")]
    public async Task<IActionResult> GetCities()
    {
        var cities = await _officeService.GetOfficeCities();

        return Ok(cities);
    }

    [HttpGet]
    public async Task<IActionResult> GetOffices(string city, string? officeType)
    {
        var offices = await _officeService.GetOffices(city, officeType);

        return Ok(offices);
    }

    [HttpGet("{officeId}/available-slots")]
    public async Task<IActionResult> GetAvailableSlots(int officeId, DateOnly date)
    {
        var slots = await _officeService.GetAvailableSlots(officeId, date);

        if (slots == null)
        {
            return NoContent();
        }

        return Ok(slots);
    }

    [HttpGet("{officeId}/schedule/{dayOfWeek}")]
    public async Task<IActionResult> GetOfficeWorkingHours(int officeId, byte dayOfWeek)
    {
        var hours = await _officeService.GetOfficeWorkingHours(officeId, dayOfWeek);

        if (hours == null)
        {
            return NoContent();
        }

        return Ok(new
        {
            OpenTime = hours.Value.OpenTime,
            CloseTime = hours.Value.CloseTime
        });
    }
}
