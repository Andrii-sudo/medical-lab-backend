using LabAPI.DTOs;

namespace LabAPI.Services;
public interface ISampleService
{
    Task UpdateExpiredSamples();
    IQueryable<SampleResponse> GetSamples();
    IQueryable<SampleResponse> GetSamplesByOrder(string orderNumber);
    IQueryable<SampleResponse> GetSamplesByPatient(string patient);
    Task<(List<SampleResponse>, int)> GetPage(IQueryable<SampleResponse> samplesQuery, int page, int pageSize);
    Task<bool> CollectSample(int sampleId);
}
