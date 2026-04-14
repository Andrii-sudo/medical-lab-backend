using Microsoft.AspNetCore.Mvc;
using LabAPI.DTOs;
using LabAPI.Models;
using Microsoft.AspNetCore.Identity;
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
        var officesId = await _context.EmployeeSchedules
            .Where(es => es.EmployeeId == employeeId)
            .Select(es => es.OfficeId)
            .Union(
                _context.EmployeeShifts
                    .Where(es => es.EmployeeId == employeeId && es.OfficeId != null)
                    .Select(es => es.OfficeId ?? -1)
            ).ToListAsync();

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
}
