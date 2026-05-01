using System.ComponentModel.DataAnnotations;

namespace LabAPI.DTOs;

public class CreatePatientRequest
{
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

    public required string Password { get; set; }
}
