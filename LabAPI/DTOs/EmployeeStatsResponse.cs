namespace LabAPI.DTOs;
    
public class EmployeeStatsResponse
{
    public required int PlannedVisitors { get; set; }
    public required int PendingSamples { get; set; }
    public required int ProcessingSamples { get; set; }
    public required int CompletedResults { get; set; }
}
