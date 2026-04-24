namespace LabAPI.DTOs;
    
public class OfficeScheduleDto
{
    public required byte DayOfWeek { get; set; }
    public required TimeOnly? OpenTime { get; set; }
    public required TimeOnly? CloseTime { get; set; }
}
