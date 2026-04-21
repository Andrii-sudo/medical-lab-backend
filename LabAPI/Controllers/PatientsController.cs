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
        var (patients, pageCount) = await _patientService.GetPatients(page, pageSize, searchTerm);

        return Ok(new 
        {
            Patients = patients, 
            PageCount = pageCount 
        });
    }

    [Authorize(Roles = $"{Roles.Admin},{Roles.Employee}")]
    [HttpGet("search")]
    public async Task<IActionResult> SearchPatients(string searchTerm, int take)
    {
        var patients = await _patientService.GetPatients(searchTerm, take);

        return Ok(patients);
    }

    [Authorize(Roles = $"{Roles.Admin},{Roles.Employee}")]
    [HttpPost]
    public async Task<IActionResult> CreatePatient(CreatePatientRequest request)
    {
        if (await _patientService.CreatePatient(request) == false)
        {
            return BadRequest(new { msg = "Пацієнт з таким номером вже існує" });
        }

        return Created();
    }

    [Authorize(Roles = $"{Roles.Admin},{Roles.Employee}")]
    [HttpPut]
    public async Task<IActionResult> UpdatePatient(UpdatePatientRequest request)
    {
        if (await _patientService.UpdatePatient(request) == false)
        {
            return BadRequest(new { msg = "Пацієнт з таким номером вже існує" });
        }

        return Ok();
    }
}
