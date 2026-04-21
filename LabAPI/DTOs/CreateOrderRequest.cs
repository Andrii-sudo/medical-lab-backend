using System.ComponentModel.DataAnnotations;

namespace LabAPI.DTOs;
public class CreateOrderRequest
{
    [Required]
    public required int PatientId { get; set; }

    [Required]
    public required int OfficeId { get; set; }

    [Required]
    public required List<int> AnalysisIds { get; set; }
}
