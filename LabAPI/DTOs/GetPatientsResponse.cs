using LabAPI.Models;

namespace LabAPI.DTOs;
public class GetPatientsResponse
{
    public required List<Patient> Patients { get; set; }
    public required int PageCount { get; set; }
}
