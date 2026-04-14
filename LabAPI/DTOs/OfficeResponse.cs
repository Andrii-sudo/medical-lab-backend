using System.ComponentModel.DataAnnotations;

namespace LabAPI.DTOs;
public class OfficeResponse
{
    public required int Id { get; set; }
    public required int Number { get; set; }
    public required string City { get; set; }
    public required string Address { get; set; }
    public required string Type { get; set; }
}
