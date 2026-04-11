using System.ComponentModel.DataAnnotations;

namespace LabAPI.DTOs;

public class LoginEmployeeRequest
{
    [Required]
    public required string Email { get; set; }

    [Required]
    public required string Password { get; set; }
}
