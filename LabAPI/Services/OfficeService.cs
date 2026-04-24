using LabAPI.Constants;
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
                && es.ShiftType == ShiftTypes.Work
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
                && (officeType == null || o.Type == officeType || o.Type == OfficeTypes.Mixed))
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

    public async Task<(List<OfficeResponse>, int)> GetOfficesPage(int page, int pageSize, string? city, string? officeType)
    {
        var query = _context.Offices.AsQueryable();

        if (!string.IsNullOrEmpty(city))
        {
            query = query.Where(o => o.City == city);
        }

        if (!string.IsNullOrEmpty(officeType))
        {
            query = query.Where(o => o.Type == officeType || o.Type == OfficeTypes.Mixed);
        }

        int totalCount = await query.CountAsync();
        int pageCount = (int)Math.Ceiling((double)totalCount / pageSize);

        var offices = await query
            .OrderBy(o => o.City)
            .ThenBy(o => o.Number)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(o => new OfficeResponse
            {
                Id = o.Id,
                Number = o.Number,
                City = o.City,
                Address = o.Address,
                Type = o.Type
            })
            .ToListAsync();

        return (offices, pageCount);
    }

    public async Task CreateOffice(CreateOfficeRequest request)
    {
        var maxNumber = await _context.Offices
            .Where(o => o.City == request.City)
            .MaxAsync(o => (short?)o.Number) ?? 0;

        var newOffice = new Office
        {
            City = request.City,
            Address = request.Address,
            Type = request.Type,
            Number = (short)(maxNumber + 1)
        };

        await _context.Offices.AddAsync(newOffice);
        await _context.SaveChangesAsync();
    }

    public async Task<List<OfficeScheduleDto>> GetOfficeSchedule(int officeId)
    {
        var schedule = await _context.OfficeSchedules
            .Where(os => os.OfficeId == officeId)
            .Select(os => new OfficeScheduleDto
            {
                DayOfWeek = os.DayOfWeek,
                OpenTime = os.OpenTime,
                CloseTime = os.CloseTime
            })
            .ToListAsync();

        return schedule;
    }

    public async Task UpdateOfficeSchedule(int officeId, List<OfficeScheduleDto> schedule)
    {
        var existingSchedule = await _context.OfficeSchedules
            .Where(os => os.OfficeId == officeId)
            .ToListAsync();

        _context.OfficeSchedules.RemoveRange(existingSchedule);

        var newSchedule = schedule.Select(s => new OfficeSchedule
        {
            OfficeId = officeId,
            DayOfWeek = s.DayOfWeek,
            OpenTime = s.OpenTime!.Value,
            CloseTime = s.CloseTime!.Value
        });

        await _context.OfficeSchedules.AddRangeAsync(newSchedule);
        await _context.SaveChangesAsync();
    }
}
