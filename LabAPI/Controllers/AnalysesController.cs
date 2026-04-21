using LabAPI.Constants;
using LabAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LabAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AnalysesController : ControllerBase
{
    private readonly IAnalysesService _analysesService;

    public AnalysesController(IAnalysesService analysesService)
    {
        _analysesService = analysesService;
    }

    [Authorize(Roles = $"{Roles.Admin},{Roles.Employee}")]
    [HttpGet("search")]
    public async Task<IActionResult> SearchAnalyses(string searchTerm, int take)
    {
        var analyses = await _analysesService.GetAnalyses(searchTerm, take);

        return Ok(analyses);
    }
}
