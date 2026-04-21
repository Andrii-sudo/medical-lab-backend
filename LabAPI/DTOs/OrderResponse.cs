namespace LabAPI.DTOs;

public class OrderResponse
{
    public required int Number { get; set; }

    public required DateOnly CreatedDate { get; set; }
    
    public required List<string> Analyses  { get; set; }
    
    public required string Status { get; set; }
    
    public required int Price { get; set; }

    public required string PatientFirstName { get; set; }
    
    public required string PatientLastName { get; set; }
    
    public required string PatientPhone { get; set; }
}
