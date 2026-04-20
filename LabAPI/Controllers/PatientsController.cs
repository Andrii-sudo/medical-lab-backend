using LabAPI.Constants;
using LabAPI.DTOs;
using LabAPI.Models;
using LabAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace LabAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PatientsController : ControllerBase
{
    private readonly IPatientService _patientService;

    public PatientsController(IPatientService patientService)
    {
        _patientService = patientService;
    }


    [Authorize(Roles = $"{Roles.Admin},{Roles.Employee}")]
    [HttpGet]
    public async Task<IActionResult> GetPatients(int page, int pageSize, string? searchTerm)
    {
        return Ok(await _patientService.GetPatients(page, pageSize, searchTerm));
    }

    [Authorize(Roles = $"{Roles.Admin},{Roles.Employee}")]
    [HttpPost("AddPatient")]
    public async Task<IActionResult> AddPatient(CreatePatientRequest request)
    {
        if (await _patientService.AddPatient(request) == false)
        {
            return BadRequest(new { msg = "Пацієнт з таким номером вже існує" });
        }

        return Created();
    }

    [Authorize(Roles = $"{Roles.Admin},{Roles.Employee}")]
    [HttpPut("EditPatient")]
    public async Task<IActionResult> EditPatient(UpdatePatientRequest request)
    {
        if (await _patientService.EditPatient(request) == false)
        {
            return BadRequest(new { msg = "Пацієнт з таким номером вже існує" });
        }

        return Ok();
    }
}
