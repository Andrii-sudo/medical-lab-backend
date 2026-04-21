namespace LabAPI.DTOs;
public class AnalysisResponse
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public required string SampleType { get; set; }
    public required byte ExpiryDays { get; set; }
    public required decimal Price {  get; set; }
}
