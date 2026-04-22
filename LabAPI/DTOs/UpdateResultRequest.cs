using System.ComponentModel.DataAnnotations;

namespace LabAPI.DTOs;
public class UpdateResultRequest
{
    [Required]
    public int Id { get; set; }
    public string? Conclusion { get; set; }

    [Required]
    public required List<UpdateResultParameterRequest> Parameters { get; set; }
}
