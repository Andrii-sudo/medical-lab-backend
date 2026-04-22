using LabAPI.Constants;
using LabAPI.DTOs;
using LabAPI.Models;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace LabAPI.Services;
public class SampleService : ISampleService
{
    private readonly MedicalLabsContext _context;
    private readonly IPatientService _patientService;
    public SampleService(MedicalLabsContext context, IPatientService patientService)
    {
        _context = context;
        _patientService = patientService;
    }

    public async Task UpdateExpiredSamples()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        
        await _context.Samples
            .Where(s => s.ExpiryDate != null 
                && s.ExpiryDate <= today
                && s.Status == "collected")
            .ExecuteUpdateAsync(s => s.SetProperty(x => x.Status, "expired"));
    }

    public IQueryable<SampleResponse> GetSamples()
    {
        var samplesQuery = _context.Samples
           .Select(s => new SampleResponse
           {
               Id = s.Id,
               Type = s.Results.Select(r => r.Analysis.SampleType).FirstOrDefault() ?? "Невідомо",
               Status = s.Status,
               CollectionDate = s.CollectionDate != null
                   ? DateOnly.FromDateTime(s.CollectionDate.Value)
                   : null,
               ExpiryDate = s.ExpiryDate,
               OrderNumber = s.OrderNumber,

               PatientFirstName = s.OrderNumberNavigation.Patient.FirstName,
               PatientLastName = s.OrderNumberNavigation.Patient.LastName,
               PatientPhone = s.OrderNumberNavigation.Patient.Phone
           }).OrderByDescending(s => s.Id);

        return samplesQuery;
    }

    public IQueryable<SampleResponse> GetSamplesByOrder(string orderNumber)
    {
        var samplesQuery = GetSamples()
            .Where(s => s.OrderNumber.ToString().Contains(orderNumber))
            .OrderByDescending(s => s.OrderNumber.ToString() == orderNumber)
            .ThenByDescending(s => s.OrderNumber.ToString().StartsWith(orderNumber))
            .ThenByDescending(s => s.Id);

        return samplesQuery;
    }

    public IQueryable<SampleResponse> GetSamplesByPatient(string patient)
    {
        var samplesQuery = _patientService.GetPatientsBySearchTerm(patient)
            .SelectMany(p => p.LabOrders)
            .SelectMany(o => o.Samples)
            .Select(s => new SampleResponse
            {
                Id = s.Id,
                Type = s.Results.Select(r => r.Analysis.SampleType).FirstOrDefault() ?? "Невідомо",
                Status = s.Status,
                CollectionDate = s.CollectionDate != null
                    ? DateOnly.FromDateTime(s.CollectionDate.Value)
                    : null,
                ExpiryDate = s.ExpiryDate,
                OrderNumber = s.OrderNumber,

                PatientFirstName = s.OrderNumberNavigation.Patient.FirstName,
                PatientLastName = s.OrderNumberNavigation.Patient.LastName,
                PatientPhone = s.OrderNumberNavigation.Patient.Phone
            })
            .OrderByDescending(s => s.Id);

        return samplesQuery;
    }

    public async Task<(List<SampleResponse>, int)> GetPage(IQueryable<SampleResponse> samplesQuery, int page, int pageSize)
    {
        int totalCount = await samplesQuery.CountAsync();
        int pageCount = (int)Math.Ceiling((double)totalCount / pageSize);

        var samples = await samplesQuery
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (samples, pageCount);
    }

    public async Task<bool> CollectSample(int sampleId)
    {
        var sample = await _context.Samples.FindAsync(sampleId);
        if (sample == null || sample.Status != SampleStatuses.Waiting)
        {
            return false;
        }

        sample.Status = SampleStatuses.Collected; 
        await _context.SaveChangesAsync(); // Дати встановить тригер

        return true;
    }
}
