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
    [HttpGet("CurrentEmployeeOffice")]
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
    [HttpGet("EmployeeOffices")]
    public async Task<IActionResult> GetEmployeeOffices(int employeeId)
    {
        var officesResponse = await _officeService.GetEmployeeOffices(employeeId);

        return Ok(officesResponse);
    }
}
