using LabAPI.Models;

namespace LabAPI.DTOs;
public class PatientsResponse
{
    public required List<Patient> Patients { get; set; }
    public required int PageCount { get; set; }
}
