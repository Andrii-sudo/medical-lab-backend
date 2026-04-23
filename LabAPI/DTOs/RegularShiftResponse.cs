namespace LabAPI.DTOs;

public class RegularShiftResponse
{
    public required int Id { get; set; }
    public required byte DayOfWeek { get; set; }
    public required string OfficeCity { get; set; }
    public required int OfficeNumber { get; set; }
    public required string OfficeAddress { get; set; }
    public required TimeOnly StartTime { get; set; }
    public required TimeOnly EndTime { get; set; }
}
