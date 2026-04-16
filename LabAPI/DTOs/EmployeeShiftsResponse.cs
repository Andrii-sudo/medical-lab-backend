namespace LabAPI.DTOs;
public class EmployeeShiftResponse
{
    public required byte DayOfWeek { get; set; }
    public string? City { get; set; }
    public int? OfficeNumber { get; set; }
    public string? LeaveReason { get; set; }
    public TimeOnly? StartTime { get; set; }
    public TimeOnly? EndTime { get; set; }
}

