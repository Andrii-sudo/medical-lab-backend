using LabAPI.Constants;
using LabAPI.Models;
using LabAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace LabAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SamplesController : ControllerBase
{
    private readonly ISampleService _sampleService;

    public SamplesController(ISampleService sampleService)
    {
        _sampleService = sampleService;
    }

    [Authorize(Roles = ($"{Roles.Admin},{Roles.Employee}"))]
    [HttpGet("by-order")]
    public async Task<IActionResult> GetSamplesByOrder(int page, int pageSize, string? orderNumber)
    {
        var query = _sampleService.GetSamples();
        if (!string.IsNullOrWhiteSpace(orderNumber))
        {
            query = _sampleService.GetSamplesByOrder(orderNumber);
        }

        var (samples, pageCount) = await _sampleService.GetPage(query, page, pageSize);

        return Ok(new
        {
            Samples = samples,
            PageCount = pageCount
        });
    }

    [Authorize(Roles = ($"{Roles.Admin},{Roles.Employee}"))]
    [HttpGet("by-patient")]
    public async Task<IActionResult> GetSamplesByPatient(int page, int pageSize, string? patient)
    {
        var query = _sampleService.GetSamples();
        if (!string.IsNullOrWhiteSpace(patient))
        {
            query = _sampleService.GetSamplesByPatient(patient);
        }

        var (samples, pageCount) = await _sampleService.GetPage(query, page, pageSize);

        return Ok(new
        {
            Samples = samples,
            PageCount = pageCount
        });
    }

    [Authorize(Roles = ($"{Roles.Admin},{Roles.Employee}"))]
    [HttpPut("{sampleId}/collect")]
    public async Task<IActionResult> CollectSample(int sampleId)
    {
        if (!await _sampleService.CollectSample(sampleId))
        {
            return BadRequest();
        }

        return Ok();
    }
}
