namespace LabAPI.DTOs;
public class ShiftResponse
{
    public required int Id { get; set; }
    public required DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public TimeOnly? StartTime { get; set; }
    public TimeOnly? EndTime { get; set; }
    public required string Type { get; set; }
    public string? officeCity { get; set; }
    public string? officeNumber { get; set; }
    public string? officeAddress { get; set; }
}
