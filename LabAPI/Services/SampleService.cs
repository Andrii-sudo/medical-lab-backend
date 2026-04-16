using LabAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace LabAPI.Services;
public class SampleService : ISampleService
{
    private readonly MedicalLabsContext _context;
    public SampleService(MedicalLabsContext context)
    {
        _context = context;
    }

    public async Task UpdateExpiredSamples()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        
        await _context.Samples
            .Where(s => s.ExpiryDate != null 
                && s.ExpiryDate <= today
                && s.Status == "collected")
            .ExecuteUpdateAsync(s => s.SetProperty(x => x.Status, "expired"));
    }
}
