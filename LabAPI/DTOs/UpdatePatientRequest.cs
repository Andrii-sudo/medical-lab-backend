using System.ComponentModel.DataAnnotations;

namespace LabAPI.DTOs;

public class UpdatePatientRequest
{
    [Required]
    public required int Id { get; set; }
    
    [Required]
    public required string FirstName { get; set; }

    [Required]
    public required string LastName { get; set; }

    public string? MiddleName { get; set; }

    [Required]
    public required DateOnly BirthDate { get; set; }

    [Required]
    public required string Gender { get; set; }

    [Required]
    public required string Phone { get; set; }

    public string? Email { get; set; }
}
