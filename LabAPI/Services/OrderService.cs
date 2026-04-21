using LabAPI.Constants;
using LabAPI.DTOs;
using LabAPI.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace LabAPI.Services;

public class OrderService: IOrderService
{
    private readonly MedicalLabsContext _context;
    private readonly IPatientService _patientService;

    public OrderService(MedicalLabsContext context, IPatientService patientService)
    {
        _context = context;
        _patientService = patientService;
    }

    public async Task<bool> CreateOrder(CreateOrderRequest request)
    {
        var analysisIdsTable = new DataTable();
        analysisIdsTable.Columns.Add("id", typeof(int));

        if (request.AnalysisIds != null)
        {
            foreach (var id in request.AnalysisIds)
            {
                analysisIdsTable.Rows.Add(id);
            }
        }

        var patientIdParam = new SqlParameter("@patient_id", request.PatientId);
        var officeIdParam = new SqlParameter("@office_id", request.OfficeId);

        var analysisIdsParam = new SqlParameter("@analysis_ids", SqlDbType.Structured)
        {
            TypeName = "dbo.int_list",
            Value = analysisIdsTable
        };

        try
        {
            await _context.Database.ExecuteSqlRawAsync(
                "EXEC create_lab_order @patient_id, @office_id, @analysis_ids",
                patientIdParam,
                officeIdParam,
                analysisIdsParam
            );

            return true;
        }
        catch (SqlException ex)
        { 
            // RAISERROR
            Console.WriteLine($"SQL Error: {ex.Message}");
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return false;
        }
    }

    public IQueryable<OrderResponse> GetOrders()
    {
        var ordersQuery = _context.LabOrders
           .Select(o => new OrderResponse
           {
               Number = o.Number,
               CreatedDate = DateOnly.FromDateTime(o.CreatedDate),
               Analyses = o.OrderAnalyses.Select(oa => oa.Analysis.Name).ToList(),
               Status = o.Status,
               Price = o.TotalPrice,

               PatientFirstName = o.Patient.FirstName,
               PatientLastName = o.Patient.LastName,
               PatientPhone = o.Patient.Phone
           })
           .OrderByDescending(o => o.CreatedDate);

        return ordersQuery;
    }

    public IQueryable<OrderResponse> GetOrdersByNumber(string number)
    {
        var ordersQuery = GetOrders()
            .Where(o => o.Number.ToString().Contains(number))
            .OrderByDescending(o => o.Number.ToString() == number)
            .ThenByDescending(o => o.Number.ToString().StartsWith(number))
            .ThenByDescending(o => o.CreatedDate);

        return ordersQuery;
    }

    public IQueryable<OrderResponse> GetOrdersByPatient(string patient)
    {
        var ordersQuery = _patientService.GetPatientsBySearchTerm(patient)
            .SelectMany(p => p.LabOrders
                .Select(o => new OrderResponse
                {
                    Number = o.Number,
                    CreatedDate = DateOnly.FromDateTime(o.CreatedDate),
                    Analyses = o.OrderAnalyses.Select(oa => oa.Analysis.Name).ToList(),
                    Status = o.Status,
                    Price = o.TotalPrice,

                    PatientFirstName = p.FirstName,
                    PatientLastName = p.LastName,
                    PatientPhone = p.Phone
                }))
            .OrderByDescending(os => os.CreatedDate);

        return ordersQuery;
    }

    public async Task<(List<OrderResponse>, int)> GetPage(IQueryable<OrderResponse> ordersQuery, int page, int pageSize)
    {
        int totalCount = await ordersQuery.CountAsync();
        int pageCount = (int)Math.Ceiling((double)totalCount / pageSize);

        var orders = await ordersQuery
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (orders, pageCount);
    }

    public async Task<bool> PayOrder(int number)
    {
        var order = await _context.LabOrders.FindAsync(number);
        if (order == null || order.Status != LabOrderStatuses.Unpaid)
        {
            return false;
        }

        order.Status = LabOrderStatuses.Pending;
        await _context.SaveChangesAsync();
        
        return true;
    }

    public async Task CancelOrder(int number)
    {
        await _context.LabOrders
            .Where(lo => lo.Number == number)
            .ExecuteUpdateAsync(lo => lo
                .SetProperty(x => x.Status, LabOrderStatuses.Cancelled));
    }
}
