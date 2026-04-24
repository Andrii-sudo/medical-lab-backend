using LabAPI.DTOs;
using LabAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace LabAPI.Services;
public class AnalysesService : IAnalysesService
{
    private readonly MedicalLabsContext _context;

    public AnalysesService(MedicalLabsContext context)
    {
        _context = context;
    }

    public IQueryable<Analysis> GetAnalysesBySearchTerm(string searchTerm)
    {
        searchTerm = searchTerm.Trim();
        IQueryable<Analysis> analyses = _context.Analyses
                .Where(a => a.Name.Contains(searchTerm))
                .OrderByDescending(a => a.Name.StartsWith(searchTerm));

        return analyses;
    }

    public async Task<List<AnalysisResponse>> GetAnalyses(string searchTerm, int take)
    {
        var query = GetAnalysesBySearchTerm(searchTerm);

        var analyses = await query
            .Select(a => new AnalysisResponse
            {
                Id = a.Id,
                Name = a.Name,
                ExpiryDays = a.ExpiryDays,
                SampleType = a.SampleType,
                Price = a.Price
            })
            .Take(take)
            .ToListAsync();

        return analyses;
    }

    public async Task<(List<AnalysisResponse>, int)> GetAnalyses(int page, int pageSize, string? searchTerm)
    {
        IQueryable<Analysis> query;
        if (!string.IsNullOrEmpty(searchTerm))
        {
            query = GetAnalysesBySearchTerm(searchTerm);
        }
        else 
        {
            query = _context.Analyses;
        }

        int totalCount = await query.CountAsync();
        int pageCount = (int)Math.Ceiling((double)totalCount / pageSize);

        var analyses = await query
           .Select(a => new AnalysisResponse
           {
               Id = a.Id,
               Name = a.Name,
               ExpiryDays = a.ExpiryDays,
               SampleType = a.SampleType,
               Price = a.Price
           })
           .Skip((page - 1) * pageSize)
           .Take(pageSize)
           .ToListAsync();

        return (analyses, totalCount);
    }
}
