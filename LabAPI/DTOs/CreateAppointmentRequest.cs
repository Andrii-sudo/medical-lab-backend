using System.ComponentModel.DataAnnotations;

namespace LabAPI.DTOs;
public class CreateAppointmentRequest
{
    [Required]
    public required int PatientId { get; set; }
    
    [Required]
    public required int OfficeId { get; set; }

    [Required]
    public required DateOnly VisitDate { get; set; }

    [Required]
    public required TimeOnly VisitTime { get; set; }

    [Required]
    public required string Purpose { get; set; }
}

