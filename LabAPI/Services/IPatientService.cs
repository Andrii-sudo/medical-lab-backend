using LabAPI.DTOs;
using LabAPI.Models;

namespace LabAPI.Services;
public interface IPatientService
{
    IQueryable<Patient> GetPatientsBySearchTerm(string searchTerm);

    Task<GetPatientsResponse> GetPatients(int page, int pageSize, string? searchTerm);
}
