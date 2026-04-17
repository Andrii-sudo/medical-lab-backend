using LabAPI.DTOs;
using LabAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace LabAPI.Services;

public class PatientService : IPatientService
{
    private readonly MedicalLabsContext _context;
    
    public PatientService(MedicalLabsContext context)
    {
        _context = context;
    }

    public IQueryable<Patient> GetPatientsBySearchTerm(string searchTerm)
    {
        searchTerm = searchTerm.Trim().ToLower();
        IQueryable<Patient> patients;

        if (searchTerm.Any(char.IsLetter))
        {
            var tokens = searchTerm.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            patients = _context.Patients
                .Where(p => tokens.All(t => p.LastName.Contains(t) || p.FirstName.Contains(t)))
                .OrderByDescending(p => p.LastName.StartsWith(searchTerm))
                    .ThenByDescending(p => p.FirstName.StartsWith(searchTerm))
                    .ThenBy(p => p.LastName)
                    .ThenBy(p => p.FirstName);
        }
        else
        {
            patients = _context.Patients
                .Where(p => p.Phone.Contains(searchTerm))
                .OrderByDescending(p => p.Phone.StartsWith(searchTerm))
                    .ThenBy(p => p.LastName);
        }

        return patients;
    }

    public async Task<GetPatientsResponse> GetPatients(int page, int pageSize, string? searchTerm)
    {
        var query = _context.Patients.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = GetPatientsBySearchTerm(searchTerm);
        }
        else
        {
            query = query.OrderByDescending(p => p.Id);
        }

        int totalCount = await query.CountAsync();

        var patients = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();


        return new GetPatientsResponse
        {
            Patients = patients,
            PageCount = (int)Math.Ceiling((double)totalCount / pageSize)
        };
    }
}

