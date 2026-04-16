namespace LabAPI.DTOs;
public class EmployeeSampleResponse
{
    public required string Type { get; set; }
    public required DateOnly expiresAt { get; set; }
    public required int OrderNumber { get; set; }
}
