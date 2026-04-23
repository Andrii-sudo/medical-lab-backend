using System.ComponentModel.DataAnnotations;

namespace LabAPI.DTOs;

public class CreateShiftRequest
{
    [Required]
    public required int EmployeeId { get; set; }
    
    [Required]
    public required string Type { get; set; }

    [Required]
    public required DateOnly StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public TimeOnly? StartTime { get; set; }

    public TimeOnly? EndTime { get; set; }

    public int? OfficeId { get; set; }

}
