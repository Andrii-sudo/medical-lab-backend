namespace LabAPI.DTOs;

public class EmployeeResponse
{
    public required int Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? MiddleName { get; set; }
    public required string Phone { get; set; }
    public required string Email { get; set; }
    public required bool IsAdmin { get; set; }
}
