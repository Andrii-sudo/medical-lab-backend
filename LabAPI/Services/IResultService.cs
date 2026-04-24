using LabAPI.DTOs;

namespace LabAPI.Services;

public interface IResultService
{
    IQueryable<ResultResponse> GetResults();
    IQueryable<ResultResponse> GetResultsByOrder(string orderNumber);
    IQueryable<ResultResponse> GetResultsByPatient(string patient);
    Task<(List<ResultResponse>, int)> GetPage(IQueryable<ResultResponse> resultsQuery, int page, int pageSize);
    Task<(List<ResultParameterResponse>, string?)> GetResultInfo(int resultId);
    Task<bool> UpdateResultInfo(UpdateResultRequest request);

    Task<(List<ResultResponse>, int)> GetMyResults(int appUserId, int page, int pageSize);
}
