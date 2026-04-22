namespace LabAPI.DTOs;
public class ResultResponse
{
    public required int Id { get; set; }
    public required string Status { get; set; }
    public required string SampleType { get; set; }
    public required string AnalysisName { get; set; }
    public required int OrderNumber { get; set; }
    public required string PatientFirstName { get; set; }
    public required string PatientLastName { get; set; }
    public required string PatientPhone { get; set; }
}
