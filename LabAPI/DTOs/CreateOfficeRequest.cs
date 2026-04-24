using System.ComponentModel.DataAnnotations;

namespace LabAPI.DTOs;
public class CreateOfficeRequest
{
    [Required]
    public required string City { get; set; }

    [Required]
    public required string Address { get; set; }

    [Required]
    public required string Type { get; set; }
}
