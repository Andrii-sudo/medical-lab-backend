namespace LabAPI.DTOs;
public class AppointmentResponse
{
    public required int Id { get; set; }
    public required TimeOnly VisitTime { get; set; }
    public required string Status { get; set; }
    public required string Purpose { get; set; }
    public required int PatientId { get; set; }
    public required string PatientFirstName { get; set; }
    public required string PatientLastName { get; set; }
    public required string PatientPhone { get; set; }
}
