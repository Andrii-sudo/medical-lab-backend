using System.ComponentModel.DataAnnotations;

namespace LabAPI.DTOs;
public class UpdateResultParameterRequest
{
    [Required]
    public required int Id { get; set; }

    [Required]
    public required decimal Value { get; set; }
}
