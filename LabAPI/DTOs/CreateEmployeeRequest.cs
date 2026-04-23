using System.ComponentModel.DataAnnotations;

namespace LabAPI.DTOs;
public class CreateEmployeeRequest
{
    [Required]
    public required string FirstName { get; set; }

    [Required]
    public required string LastName { get; set; }

    public string? MiddleName { get; set; }

    [Required]
    public required string Phone { get; set; }
    
    [Required]
    public required string Email { get; set; }

    [Required]
    public required string Password { get; set; }
    
    [Required]
    public required bool IsAdmin { get; set; }
}
