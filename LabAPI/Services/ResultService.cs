using LabAPI.Constants;
using LabAPI.DTOs;
using LabAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace LabAPI.Services;
public class ResultService : IResultService
{
    private readonly MedicalLabsContext _context;
    private readonly IPatientService _patientService;

    public ResultService(MedicalLabsContext context, IPatientService patientService)
    {
        _context = context;
        _patientService = patientService;
    }

    public IQueryable<ResultResponse> GetResults()
    {
        var resultsQuery = _context.Results
           .Where(r => r.Sample.Status != SampleStatuses.Waiting && r.Sample.Status != SampleStatuses.Expired)
           .Select(r => new ResultResponse
           {
               Id = r.Id,
               Status = r.Status,
               SampleType = r.Analysis.SampleType,
               AnalysisName = r.Analysis.Name,
               OrderNumber = r.Sample.OrderNumber,

               PatientFirstName = r.Sample.OrderNumberNavigation.Patient.FirstName,
               PatientLastName = r.Sample.OrderNumberNavigation.Patient.LastName,
               PatientPhone = r.Sample.OrderNumberNavigation.Patient.Phone
           }).OrderByDescending(r => r.Id);

        return resultsQuery;
    }

    public IQueryable<ResultResponse> GetResultsByOrder(string orderNumber)
    {
        var resultsQuery = GetResults()
            .Where(r => r.OrderNumber.ToString().Contains(orderNumber))
            .OrderByDescending(r => r.OrderNumber.ToString() == orderNumber)
            .ThenByDescending(r => r.OrderNumber.ToString().StartsWith(orderNumber))
            .ThenByDescending(r => r.Id);

        return resultsQuery;
    }

    public IQueryable<ResultResponse> GetResultsByPatient(string patient)
    {
        var resultsQuery = _patientService.GetPatientsBySearchTerm(patient)
            .SelectMany(p => p.LabOrders)
            .SelectMany(o => o.Samples)
            .Where(s => s.Status != SampleStatuses.Waiting && s.Status != SampleStatuses.Expired)
            .SelectMany(s => s.Results)
            .Select(r => new ResultResponse
            {
                Id = r.Id,
                SampleType = r.Analysis.SampleType,
                Status = r.Status,
                AnalysisName = r.Analysis.Name,
                OrderNumber = r.Sample.OrderNumber,

                PatientFirstName = r.Sample.OrderNumberNavigation.Patient.FirstName,
                PatientLastName = r.Sample.OrderNumberNavigation.Patient.LastName,
                PatientPhone = r.Sample.OrderNumberNavigation.Patient.Phone
            })
            .OrderByDescending(s => s.Id);

        return resultsQuery;
    }

    public async Task<(List<ResultResponse>, int)> GetPage(IQueryable<ResultResponse> resultsQuery, int page, int pageSize)
    {
        int totalCount = await resultsQuery.CountAsync();
        int pageCount = (int)Math.Ceiling((double)totalCount / pageSize);

        var results = await resultsQuery
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (results, pageCount);
    }

    public async Task<List<ResultParameterResponse>> GetResultParameters(int resultId)
    {
        var patientInfo = await _context.Results
            .Where(r => r.Id == resultId)
            .Select(r => new
            {
                BirthDate = r.Sample.OrderNumberNavigation.Patient.BirthDate,
                Gender = r.Sample.OrderNumberNavigation.Patient.Gender
            })
            .FirstAsync();

        var today = DateOnly.FromDateTime(DateTime.Today);

        int age = today.Year - patientInfo.BirthDate.Year;
        if (patientInfo.BirthDate > today.AddYears(-age))
        {
            age--;
        }
        string gender = patientInfo.Gender;

        var resultParameters =  await _context.ParameterResults
            .Where(pr => pr.ResultId == resultId)
            .Select(pr => new ResultParameterResponse
            { 
                Id = pr.Id,
                Name = pr.Parameter.ParameterName,
                Value = pr.MeasuredValue,
                Unit = pr.Parameter.Unit,
                NormMin = pr.Parameter.ParameterNorms
                    .Where(pn => pn.AgeMin <= age && pn.AgeMax >= age
                        && (pn.Gender == "A" || pn.Gender == gender))
                    .Select(pn => pn.MinValue).FirstOrDefault(),
                NormMax = pr.Parameter.ParameterNorms
                    .Where(pn => pn.AgeMin <= age && pn.AgeMax >= age
                        && (pn.Gender == "A" || pn.Gender == gender))
                    .Select(pn => pn.MaxValue).FirstOrDefault()
            })
            .ToListAsync();

        return resultParameters;
    }

    public async Task<bool> UpdateResultParameters(List<UpdateResultParameterRequest> request)
    {
        if (!request.Any()) return false;

        var ids = request.Select(r => r.Id).ToList();

        var paramsToUpdate = await _context.ParameterResults
            .Include(pr => pr.Result)
                .ThenInclude(r => r.Sample)
                    .ThenInclude(s => s.OrderNumberNavigation)
                        .ThenInclude(o => o.Patient)
            .Include(pr => pr.Parameter)
                .ThenInclude(p => p.ParameterNorms)
            .Where(pr => ids.Contains(pr.Id))
            .ToListAsync();

        if (!paramsToUpdate.Any()) return false;

        var result = paramsToUpdate.First().Result;
        var patient = result.Sample.OrderNumberNavigation.Patient;

        var today = DateOnly.FromDateTime(DateTime.Today);
        int age = today.Year - patient.BirthDate.Year;

        if (patient.BirthDate > today.AddYears(-age))
        {
            age--;
        }

        string gender = patient.Gender;
        bool hasAbnormal = false;

        foreach (var p in paramsToUpdate)
        {
            var reqValue = request.First(r => r.Id == p.Id).Value;
            p.MeasuredValue = reqValue;

            var norm = p.Parameter.ParameterNorms
                .FirstOrDefault(pn => pn.AgeMin <= age && pn.AgeMax >= age
                                   && (pn.Gender == "A" || pn.Gender == gender));

            bool isNormal = true;

            if (norm != null)
            {
                if (norm.MinValue.HasValue && reqValue < norm.MinValue.Value) isNormal = false;
                if (norm.MaxValue.HasValue && reqValue > norm.MaxValue.Value) isNormal = false;
            }

            p.IsNormal = isNormal;

            if (!isNormal)
            {
                hasAbnormal = true;
            }
        }

        result.Status = hasAbnormal ? ResultStatuses.Abnormal : ResultStatuses.Normal;
        result.ResultDate = DateTime.Now;

        await _context.SaveChangesAsync();
        
        return true;
    }
}
