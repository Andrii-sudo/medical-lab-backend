using LabAPI.DTOs;
using LabAPI.Models;

namespace LabAPI.Services;
public interface IAnalysesService
{
    IQueryable<Analysis> GetAnalysesBySearchTerm(string searchTerm);
    Task<List<AnalysisResponse>> GetAnalyses(string searchTerm, int take);

}
