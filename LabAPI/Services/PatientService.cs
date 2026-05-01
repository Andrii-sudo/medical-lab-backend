using LabAPI.Constants;
using LabAPI.DTOs;
using LabAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LabAPI.Services;

public class PatientService : IPatientService
{
    private readonly MedicalLabsContext _context;
    private readonly UserManager<AppUser> _userManager;

    public PatientService(MedicalLabsContext context, UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
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

    public async Task<(List<PatientResponse>, int)> GetPatients(int page, int pageSize, string? searchTerm)
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
        int pageCount = (int)Math.Ceiling((double)totalCount / pageSize);
        
        var patients = await query
            .Select(p => new PatientResponse
            {
                Id = p.Id,
                FirstName = p.FirstName,
                LastName = p.LastName,
                MiddleName = p.MiddleName,
                BirthDate = p.BirthDate,
                Gender = p.Gender,
                Phone = p.Phone,
                Email = p.Email
            })
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();


        return (patients, pageCount);
    }

    public async Task<List<PatientResponse>> GetPatients(string searchTerm, int take)
    {
        var query = GetPatientsBySearchTerm(searchTerm);

        var patients = await query
            .Select(p => new PatientResponse
            {
                Id = p.Id,
                FirstName = p.FirstName,
                LastName = p.LastName,
                MiddleName = p.MiddleName,
                BirthDate = p.BirthDate,
                Gender = p.Gender,
                Phone = p.Phone,
                Email = p.Email
            })
            .Take(take)
            .ToListAsync();

        return patients;
    }

    public async Task<bool> CreatePatient(CreatePatientRequest request)
    {
        if (await _context.Patients
            .Where(p => p.Phone == request.Phone)
            .FirstOrDefaultAsync() != null)
        {
            return false;
        }

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var patient = new Patient
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                MiddleName = request.MiddleName,
                Gender = request.Gender,
                BirthDate = request.BirthDate,
                Phone = request.Phone,
                Email = request.Email
            };
            await _context.Patients.AddAsync(patient);
            await _context.SaveChangesAsync();

            var user = new AppUser
            {
                UserName = request.Phone,
                PhoneNumber = request.Phone,
                PatientId = patient.Id,
                SecurityStamp = Guid.NewGuid().ToString(),
                ConcurrencyStamp = Guid.NewGuid().ToString()
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                await transaction.RollbackAsync();
                return false;
            }

            await _userManager.AddToRoleAsync(user, Roles.Patient);
            await transaction.CommitAsync();
            return true;
        }
        catch
        {
            await transaction.RollbackAsync();
            return false;
        }
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

