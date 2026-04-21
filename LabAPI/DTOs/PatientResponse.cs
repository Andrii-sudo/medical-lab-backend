using LabAPI.Models;

namespace LabAPI.DTOs;
public class PatientResponse
{
    public required int Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? MiddleName { get; set; }
    public required DateOnly BirthDate { get; set; }
    public required string Gender { get; set; }
    public required string Phone { get; set; }
    public string? Email { get; set; }
}
