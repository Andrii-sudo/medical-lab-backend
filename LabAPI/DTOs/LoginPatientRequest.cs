using System.ComponentModel.DataAnnotations;

namespace LabAPI.DTOs;

public class LoginPatientRequest
{
    [Required]
    public required string Phone { get; set; }

    [Required]
    public required string Password { get; set; }
}

