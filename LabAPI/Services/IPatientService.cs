using LabAPI.DTOs;
using LabAPI.Models;

namespace LabAPI.Services;
public interface IPatientService
{
    IQueryable<Patient> GetPatientsBySearchTerm(string searchTerm);
    Task<(List<PatientResponse>, int)> GetPatients(int page, int pageSize, string? searchTerm);
    Task<List<PatientResponse>> GetPatients(string searchTerm, int take);
    Task<bool> CreatePatient(CreatePatientRequest request);
    Task<bool> UpdatePatient(UpdatePatientRequest request);
}
