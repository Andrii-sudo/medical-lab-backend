using LabAPI.Constants;
using LabAPI.DTOs;
using LabAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LabAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ResultsController : ControllerBase
{
    private readonly IResultService _resultService;

    public ResultsController(IResultService resultService)
    {
        _resultService = resultService;
    }


    [Authorize(Roles = ($"{Roles.Admin},{Roles.Employee}"))]
    [HttpGet("by-order")]
    public async Task<IActionResult> GetResultsByOrder(int page, int pageSize, string? orderNumber)
    {
        var query = _resultService.GetResults();
        if (!string.IsNullOrWhiteSpace(orderNumber))
        {
            query = _resultService.GetResultsByOrder(orderNumber);
        }

        var (results, pageCount) = await _resultService.GetPage(query, page, pageSize);

        return Ok(new
        {
            Results = results,
            PageCount = pageCount
        });
    }

    [Authorize(Roles = ($"{Roles.Admin},{Roles.Employee}"))]
    [HttpGet("by-patient")]
    public async Task<IActionResult> GetResultsByPatient(int page, int pageSize, string? patient)
    {
        var query = _resultService.GetResults();
        if (!string.IsNullOrWhiteSpace(patient))
        {
            query = _resultService.GetResultsByPatient(patient);
        }

        var (results, pageCount) = await _resultService.GetPage(query, page, pageSize);

        return Ok(new
        {
            Results = results,
            PageCount = pageCount
        });
    }

    [Authorize]
    [HttpGet("{resultId}")]
    public async Task<IActionResult> GetResultInfo(int resultId)
    {
        var (parameters, conclusion) = await _resultService.GetResultInfo(resultId);

        return Ok(new
        {
            Parameters = parameters,
            Conclusion = conclusion
        });
    }

    [Authorize(Roles = ($"{Roles.Admin},{Roles.Employee}"))]
    [HttpPut()]
    public async Task<IActionResult> UpdateResultInfo(UpdateResultRequest request)
    {
        if (!await _resultService.UpdateResultInfo(request))
        {
            return BadRequest();
        }
        return Ok();
    }

    [Authorize(Roles = Roles.Patient)]
    [HttpGet("my")]
    public async Task<IActionResult> GetMyResults(int page, int pageSize)
    {
        // Безпечно дістаємо ID пацієнта прямо з токена авторизації
        var appIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(appIdStr, out int patientId))
        {
            return Unauthorized();
        }

        var (results, pageCount) = await _resultService.GetMyResults(patientId, page, pageSize);

        return Ok(new
        {
            Results = results,
            PageCount = pageCount
        });
    }
}
