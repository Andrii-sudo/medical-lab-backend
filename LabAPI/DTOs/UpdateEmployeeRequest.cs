using System.ComponentModel.DataAnnotations;

namespace LabAPI.DTOs;
public class UpdateEmployeeRequest
{
    [Required]
    public required int Id { get; set; }

    [Required]
    public required string FirstName { get; set; }

    [Required]
    public required string LastName { get; set; }

    public string? MiddleName { get; set; }

    [Required]
    public required string Phone { get; set; }

    [Required]
    public required string Email { get; set; }
}
