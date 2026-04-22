namespace LabAPI.DTOs;
public class SampleResponse
{
    public required int Id { get; set; }
    public required string Type { get; set; }
    public required string Status { get; set; }
    public DateOnly? CollectionDate { get; set; }
    public DateOnly? ExpiryDate { get; set; }
    public required int OrderNumber { get; set; }
    public required string PatientFirstName { get; set; }
    public required string PatientLastName { get; set; }
    public required string PatientPhone { get; set; }
}
