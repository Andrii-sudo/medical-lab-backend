using LabAPI.DTOs;
using LabAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace LabAPI.Services;

public class OfficeService: IOfficeService
{
    private readonly MedicalLabsContext _context;
    public OfficeService(MedicalLabsContext context)
    {
        _context = context;
    }

    public async Task<OfficeResponse?> GetCurrentEmployeeOffice(int employeeId)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var now = TimeOnly.FromDateTime(DateTime.Now);

        var employeeShift = await _context.EmployeeShifts
            .SingleOrDefaultAsync(es => es.EmployeeId == employeeId
                && es.ShiftType == "work"
                && es.ShiftDate == today
                && es.StartTime <= now
                && es.EndTime >= now);

        var employeeSchedule = await _context.EmployeeSchedules
            .SingleOrDefaultAsync(es => es.EmployeeId == employeeId
                && es.DayOfWeek == (byte)today.DayOfWeek
                && es.StartTime <= now
                && es.EndTime >= now);

        int officeId;
        if (employeeShift != null && employeeShift.OfficeId != null)
        {
            officeId = employeeShift.OfficeId ?? -1;
        }
        else if (employeeSchedule != null)
        {
            officeId = employeeSchedule.OfficeId;
        }
        else
        {
            return null;
        }

        var office = await _context.Offices.FindAsync(officeId);

        return new OfficeResponse
        {
            Id = office!.Id,
            Number = office.Number,
            City = office.City,
            Address = office.Address,
            Type = office.Type
        };
    }

    public async Task<List<OfficeResponse>> GetEmployeeOffices(int employeeId)
    {
        var officesId = _context.EmployeeSchedules
            .Where(es => es.EmployeeId == employeeId)
            .Select(es => es.OfficeId)
            .Union(
                _context.EmployeeShifts
                    .Where(es => es.EmployeeId == employeeId && es.OfficeId != null)
                    .Select(es => es.OfficeId ?? -1) // nullable
            );

        var offices = (await _context.Offices
            .Where(o => officesId.Contains(o.Id))
            .Select(o => new OfficeResponse
            {
                Id = o.Id,
                Number = o.Number,
                City = o.City,
                Address = o.Address,
                Type = o.Type
            }).ToListAsync());

        return offices;
    }

    public async Task<List<string>> GetOfficeCities()
    {
        var officeCities = await _context.Offices
            .Select(o => o.City)
            .Distinct()
            .ToListAsync();

        return officeCities;
    }

    public async Task<List<OfficeByCityResponse>> GetOffices(string city, string? officeType = null)
    {
        var offices = await _context.Offices
            .Where(o => o.City == city
                && (officeType == null || o.Type == officeType || o.Type == "mixed"))
            .Select(o => new OfficeByCityResponse
            {
                Id = o.Id,
                Number = o.Number,
                Address = o.Address
            }).ToListAsync();

        return offices;
    }

    public async Task<List<TimeOnly>?> GetAvailableSlots(int officeId, DateOnly date, int slotSpace = 15)
    {
        var schedule = await _context.OfficeSchedules
            .Where(os => os.OfficeId == officeId 
                && os.DayOfWeek == (byte)date.DayOfWeek)
            .FirstOrDefaultAsync();

        if (schedule == null) return null;

        var takenSlots = await _context.Appointments
            .Where(a => a.OfficeId == officeId
                && a.VisitDate == date)
            .Select(a => a.VisitTime)
            .ToListAsync();


        var slots = new List<TimeOnly>();
        var current = schedule.OpenTime;
        var now = TimeOnly.FromDateTime(DateTime.Now);

        while (current < schedule.CloseTime)
        { 
            if (!takenSlots.Contains(current)
                && (date > DateOnly.FromDateTime(DateTime.Today) || current > now))
            {
                slots.Add(current);
            }

            current = current.AddMinutes(slotSpace);
        }

        return slots;
    }

    public async Task<(TimeOnly OpenTime, TimeOnly CloseTime)?> GetOfficeWorkingHours(int officeId, byte dayOfWeek)
    {
        var schedule = await _context.OfficeSchedules
            .Where(os => os.OfficeId == officeId && os.DayOfWeek == dayOfWeek)
            .Select(os => new
            {
                OpenTime = os.OpenTime,
                CloseTime = os.CloseTime
            })
            .FirstOrDefaultAsync();

        return schedule != null ? (schedule.OpenTime, schedule.CloseTime) : null;
    }
}
