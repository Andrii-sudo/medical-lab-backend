using System.ComponentModel.DataAnnotations;

namespace LabAPI.DTOs;
public class CreateRegularShiftRequest
{
    [Required]
    public required int EmployeeId { get; set; }

    [Required]
    public required byte DayOfWeek { get; set; }
    
    [Required]
    public required int OfficeId { get; set; }
    
    [Required]
    public required TimeOnly StartTime { get; set; }
   
    [Required]
    public required TimeOnly EndTime { get; set; }  
}
