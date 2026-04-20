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
                .Where(p => tokens.All(t => 
                    p.LastName.Contains(t) 
                    || p.FirstName.Contains(t)))
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

    public async Task<PatientsResponse> GetPatients(int page, int pageSize, string? searchTerm)
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


        return new PatientsResponse
        {
            Patients = patients,
            PageCount = (int)Math.Ceiling((double)totalCount / pageSize)
        };
    }

    public async Task<bool> CreatePatient(CreatePatientRequest request)
    {
        if (await _context.Patients
            .Where(p => p.Phone == request.Phone)
            .FirstOrDefaultAsync() != null)
        {
            return false;
        }

        await _context.Patients.AddAsync(new Patient 
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            MiddleName = request.MiddleName,
            Gender = request.Gender,
            BirthDate = request.BirthDate,
            Phone = request.Phone,
            Email = request.Email
        });
        await _context.SaveChangesAsync();
        
        return true;
    }

    public async Task<bool> UpdatePatient(UpdatePatientRequest request)
    {
        if (await _context.Patients
            .Where(p => p.Id != request.Id
                && p.Phone == request.Phone)
            .FirstOrDefaultAsync() != null)
        {
            return false;
        }

        await _context.Patients
            .Where(p => p.Id == request.Id)
            .ExecuteUpdateAsync(p => p
                .SetProperty(x => x.FirstName, request.FirstName)
                .SetProperty(x => x.LastName, request.LastName)
                .SetProperty(x => x.MiddleName, request.MiddleName)
                .SetProperty(x => x.Gender, request.Gender)
                .SetProperty(x => x.BirthDate, request.BirthDate)
                .SetProperty(x => x.Phone, request.Phone)
                .SetProperty(x => x.Email, request.Email));

        return true;
    }
}

