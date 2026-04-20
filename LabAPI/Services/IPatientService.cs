using LabAPI.DTOs;
using LabAPI.Models;

namespace LabAPI.Services;
public interface IPatientService
{
    IQueryable<Patient> GetPatientsBySearchTerm(string searchTerm);
    Task<PatientsResponse> GetPatients(int page, int pageSize, string? searchTerm);
    Task<bool> AddPatient(CreatePatientRequest request);
    Task<bool> EditPatient(UpdatePatientRequest request);
}
