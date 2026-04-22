namespace LabAPI.DTOs;

public class ResultParameterResponse
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public decimal? Value { get; set; }
    public string? Unit { get; set; }
    public decimal? NormMin { get; set; }
    public decimal? NormMax { get; set; }
}
