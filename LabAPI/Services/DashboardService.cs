using LabAPI.DTOs;
using LabAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;

namespace LabAPI.Services;

public class DashboardService : IDashboardService
{
    private readonly MedicalLabsContext _context;

    public DashboardService(MedicalLabsContext context)
    {
        _context = context;
    }

    public async Task<int> GetPlannedVisitors(int officeId)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        
        int visitors = await _context.Appointments
            .Where(a => a.OfficeId == officeId 
                && a.Status != "cancelled"
                && a.VisitDate == today)
            .CountAsync();

        return visitors;
    }
    public async Task<int> GetPendingSamples(int officeId)
    {
        int samples = await _context.Samples
            .Where(s => s.Status == "waiting"
                && s.OrderNumberNavigation.OfficeId == officeId)
            .CountAsync();

        return samples;
    }
    public async Task<int> GetProcessingSamples(int officeId)
    {
        int samples = await _context.Samples
            .Where(s => s.Status == "collected"
                && s.OrderNumberNavigation.OfficeId == officeId)
            .CountAsync();

        return samples;
    }
    public async Task<int> GetCompletedResults(int officeId)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);

        int results = await _context.Results
            .Where(r => r.Status != "pending"
                && r.ResultDate.HasValue
                && DateOnly.FromDateTime(r.ResultDate.Value) == today
                && r.Sample.OrderNumberNavigation.OfficeId == officeId)
            .CountAsync();

        return results;
    }

    public async Task<List<EmployeeShiftResponse>> GetEmployeeSchedule(int employeeId, int dayRange = 7)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var endDate = today.AddDays(dayRange);

        var employeeSchedule = await _context.EmployeeSchedules
            .Where(es => es.EmployeeId == employeeId)
            .Select(es => new
            {
                City = es.Office.City,
                OfficeNumber = es.Office.Number,
                DayOfWeek = es.DayOfWeek,
                StartTime = es.StartTime,
                EndTime = es.EndTime
            })
            .ToListAsync();

        var employeeShifts = await _context.EmployeeShifts
            .Where(es => es.EmployeeId == employeeId
                && es.ShiftDate >= today && es.ShiftDate <= endDate)
            .OrderBy(es => es.ShiftDate)
            .Select(es => new
            {
                City = es.ShiftType == "work" ? es.Office!.City : null,
                OfficeNumber = es.ShiftType == "work" ? es.Office!.Number : -1,
                ShiftType = es.ShiftType,
                Date = es.ShiftDate,
                StartTime = es.StartTime,
                EndTime = es.EndTime
            })
            .ToListAsync();

        var result = new List<EmployeeShiftResponse>();
        for (int i = 0; i < dayRange; i++)
        {
            var currentDate = today.AddDays(i);
            var currentDayOfWeek = currentDate.DayOfWeek;

            // чи є для цієї дати виняток
            var overrideShift = employeeShifts.FirstOrDefault(es => es.Date == currentDate);

            if (overrideShift != null)
            {
                int? officeNumber = overrideShift.OfficeNumber == -1
                    ? null : overrideShift.OfficeNumber;

                var leaveReason = GetLeaveReason(overrideShift.ShiftType);

                result.Add(new EmployeeShiftResponse
                {
                    DayOfWeek = (byte)overrideShift.Date.DayOfWeek,
                    City = overrideShift.City,
                    OfficeNumber = officeNumber,
                    LeaveReason = leaveReason,
                    StartTime = overrideShift.StartTime,
                    EndTime = overrideShift.EndTime
                });
            }
            else
            {
                var regularShift = employeeSchedule.FirstOrDefault(es => es.DayOfWeek == (byte)currentDayOfWeek);

                if (regularShift != null)
                {
                    result.Add(new EmployeeShiftResponse
                    {
                        DayOfWeek = regularShift.DayOfWeek,
                        City = regularShift.City,
                        OfficeNumber = regularShift.OfficeNumber,
                        LeaveReason = null,
                        StartTime = regularShift.StartTime,
                        EndTime = regularShift.EndTime
                    });
                }
                else
                {
                    result.Add(new EmployeeShiftResponse
                    {
                        DayOfWeek = (byte)currentDayOfWeek,
                        City = null,
                        OfficeNumber = null,
                        LeaveReason = GetLeaveReason("day_off"),
                        StartTime = null,
                        EndTime = null
                    });
                }
            }
        }

        return result;


        static string? GetLeaveReason(string shiftType)
        {
            switch (shiftType)
            {
                case "day_off":
                    return "Вихідний";
                case "sick_leave":
                    return "Лікарняний";
                case "vacation":
                    return "Відпустка";
                default:
                    return null;
            }
        }
    }

    public async Task<List<EmployeeSampleResponse>> GetEmployeeSamples(int officeId, int count = 7)
    {
        var employeeSamples = await _context.Samples
            .Where(s => s.OrderNumberNavigation.OfficeId == officeId
                && s.Status == "collected"
                && s.ExpiryDate != null)
            .OrderBy(s => s.ExpiryDate)
            .Select(s => new EmployeeSampleResponse
            {
                Type = s.Results
                    .Select(oa => oa.Analysis.SampleType)
                    .FirstOrDefault() ?? "Невідомо",
                expiresAt = s.ExpiryDate!.Value,
                OrderNumber = s.OrderNumber
            })
            .Take(count)
            .ToListAsync();

        return employeeSamples;
    }
}
