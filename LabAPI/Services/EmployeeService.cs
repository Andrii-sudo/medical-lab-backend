using LabAPI.Constants;
using LabAPI.DTOs;
using LabAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LabAPI.Services;
public class EmployeeService : IEmployeeService
{
    private readonly MedicalLabsContext _context;

    private readonly UserManager<AppUser> _userManager;
    public EmployeeService(MedicalLabsContext context, UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public IQueryable<Employee> GetEmployeesBySearchTerm(string searchTerm)
    {
        searchTerm = searchTerm.Trim().ToLower();
        IQueryable<Employee> employees;

        if (searchTerm.Any(char.IsLetter))
        {
            var tokens = searchTerm.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            employees = _context.Employees
                .Where(p => tokens.All(t =>
                    p.LastName.Contains(t)
                    || p.FirstName.Contains(t)))
                .OrderByDescending(p => p.LastName.StartsWith(searchTerm))
                    .ThenByDescending(p => p.FirstName.StartsWith(searchTerm))
                    .ThenBy(p => p.LastName)
                    .ThenBy(p => p.FirstName);
        }
        else
        {
            employees = _context.Employees
                .Where(p => p.Phone.Contains(searchTerm))
                .OrderByDescending(p => p.Phone.StartsWith(searchTerm))
                    .ThenBy(p => p.LastName);
        }

        return employees;
    }

    public async Task<(List<EmployeeResponse>, int)> GetEmployees(int page, int pageSize, string? searchTerm)
    {
        var query = _context.Employees.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = GetEmployeesBySearchTerm(searchTerm);
        }
        else
        {
            query = query.OrderByDescending(p => p.Id);
        }

        int totalCount = await query.CountAsync();
        int pageCount = (int)Math.Ceiling((double)totalCount / pageSize);

        var employees = await query
            .Select(e => new EmployeeResponse
            {
                Id = e.Id,
                FirstName = e.FirstName,
                LastName = e.LastName,
                MiddleName = e.MiddleName,
                Phone = e.Phone,
                Email = e.Email,
                IsAdmin = _context.Users.Any(u => u.EmployeeId == e.Id &&
                    _context.UserRoles.Any(ur => ur.UserId == u.Id &&
                        _context.Roles.Any(r => r.Id == ur.RoleId && r.Name == Roles.Admin)))
            })
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();


        return (employees, pageCount);
    }

    public async Task<string?> CreateEmployee(CreateEmployeeRequest request)
    {
        if (await _context.Employees
            .Where(e => e.Phone == request.Phone)
            .FirstOrDefaultAsync() != null)
        {
            return "Працівник з таким номером вже існує";
        }
        else if (await _context.Employees
            .Where(e => e.Email == request.Email)
            .FirstOrDefaultAsync() != null)
        {
            return "Працівник з таким email вже існує";
        }

        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var employee = new Employee
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                MiddleName = request.MiddleName,
                Phone = request.Phone,
                Email = request.Email
            };
            await _context.Employees.AddAsync(employee);
            await _context.SaveChangesAsync();

            var user = new AppUser
            {
                UserName = request.Email,
                Email = request.Email,
                EmployeeId = employee.Id,
                SecurityStamp = Guid.NewGuid().ToString(),
                ConcurrencyStamp = Guid.NewGuid().ToString()
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                await transaction.RollbackAsync();
                return string.Join(", ", result.Errors.Select(e => e.Description));
            }

            string role = request.IsAdmin ? Roles.Admin : Roles.Employee;
            var roleResult = await _userManager.AddToRoleAsync(user, role);

            if (!roleResult.Succeeded)
            {
                await transaction.RollbackAsync();
                return "Сталася помилка на сервері";
            }

            await transaction.CommitAsync();
            return null;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            return "Сталася помилка на сервері";
        }
    }

    public async Task<string?> UpdateEmployee(UpdateEmployeeRequest request)
    {
        if (await _context.Employees
            .Where(e => e.Phone == request.Phone && e.Id != request.Id)
            .FirstOrDefaultAsync() != null)
        {
            return "Працівник з таким номером вже існує";
        }
        else if (await _context.Employees
            .Where(e => e.Email == request.Email && e.Id != request.Id)
            .FirstOrDefaultAsync() != null)
        {
            return "Працівник з таким email вже існує";
        }

        await _context.Employees
            .Where(e => e.Id == request.Id)
            .ExecuteUpdateAsync(e => e
                .SetProperty(x => x.FirstName, request.FirstName)
                .SetProperty(x => x.LastName, request.LastName)
                .SetProperty(x => x.MiddleName, request.MiddleName)
                .SetProperty(x => x.Phone, request.Phone)
                .SetProperty(x => x.Email, request.Email));

        return null;
    }

    public async Task<bool> DeleteEmployee(int employeeId)
    {
        var employee = await _context.Employees
            .FirstOrDefaultAsync(e => e.Id == employeeId);

        if (employee == null)
        {
            return false;
        }

        _context.Employees.Remove(employee);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<List<RegularShiftResponse?>> GetRegularSchedule(int employeeId)
    {
        var schedules = await _context.EmployeeSchedules
            .Include(es => es.Office)
            .Where(es => es.EmployeeId == employeeId)
            .ToListAsync();

        var result = new RegularShiftResponse?[7];

        foreach (var s in schedules)
        {
            result[s.DayOfWeek] = new RegularShiftResponse
            {
                Id = s.Id,
                DayOfWeek = (byte)s.DayOfWeek,
                OfficeCity = s.Office.City,
                OfficeNumber = s.Office.Number,
                OfficeAddress = s.Office.Address,
                StartTime = s.StartTime,
                EndTime = s.EndTime
            };
            
        }

        return result.ToList();
    }

    public async Task<bool> DeleteRegularShift(int regularShiftId)
    {
        var regularShift = await _context.EmployeeSchedules
            .FirstOrDefaultAsync(es => es.Id == regularShiftId);
    
        if (regularShift == null)
        {
            return false;
        }

        _context.EmployeeSchedules.Remove(regularShift);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task CreateRegularShift(CreateRegularShiftRequest request)
    {
        await _context.EmployeeSchedules.AddAsync(new EmployeeSchedule
        {
            EmployeeId = request.EmployeeId,
            DayOfWeek = request.DayOfWeek,
            OfficeId = request.OfficeId,
            StartTime = request.StartTime,
            EndTime = request.EndTime
        });
        await _context.SaveChangesAsync();
    }

    public async Task<bool> UpdateRegularShift(UpdateRegularShiftRequest request)
    {
        var regularShift = await _context.EmployeeSchedules
            .FirstOrDefaultAsync(es => es.Id == request.Id);

        if (regularShift == null)
        {
            return false;
        }

        regularShift.DayOfWeek = request.DayOfWeek;
        regularShift.OfficeId = request.OfficeId;
        regularShift.StartTime = request.StartTime;
        regularShift.EndTime = request.EndTime;
        await _context.SaveChangesAsync();

        return true;
    }


    public async Task<(List<ShiftResponse>, int)> GetShifts(int employeeId, int page, int pageSize, bool includePast)
    {
        var query = _context.EmployeeShifts
            .Include(es => es.Office)
            .Where(es => es.EmployeeId == employeeId);

        if (!includePast)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            query = query.Where(es => es.ShiftDate >= today);
        }

        var rawShifts = await query
            .OrderBy(es => es.ShiftDate)
            .ToListAsync();

        var aggregatedShifts = new List<ShiftResponse>();
        ShiftResponse? currentAbsence = null;

        foreach (var shift in rawShifts)
        {
            bool isWorkShift = shift.ShiftType == ShiftTypes.Work;

            if (isWorkShift)
            {
                if (currentAbsence != null)
                {
                    aggregatedShifts.Add(currentAbsence);
                    currentAbsence = null;
                }

                aggregatedShifts.Add(new ShiftResponse
                {
                    Id = shift.Id,
                    StartDate = shift.ShiftDate,
                    EndDate = null,
                    StartTime = shift.StartTime,
                    EndTime = shift.EndTime,
                    Type = shift.ShiftType,
                    officeCity = shift.Office?.City,
                    officeNumber = shift.Office?.Number.ToString(),
                    officeAddress = shift.Office?.Address
                });
            }
            else
            {
                if (currentAbsence == null)
                {
                    currentAbsence = new ShiftResponse
                    {
                        Id = shift.Id, // first day
                        StartDate = shift.ShiftDate,
                        EndDate = null,
                        Type = shift.ShiftType
                    };
                }
                else
                {
                    var lastDate = currentAbsence.EndDate ?? currentAbsence.StartDate;

                    if (shift.ShiftType == currentAbsence.Type && shift.ShiftDate == lastDate.AddDays(1))
                    {
                        currentAbsence.EndDate = shift.ShiftDate;
                    }
                    else
                    {
                        aggregatedShifts.Add(currentAbsence);

                        currentAbsence = new ShiftResponse
                        {
                            Id = shift.Id,
                            StartDate = shift.ShiftDate,
                            EndDate = null,
                            Type = shift.ShiftType
                        };
                    }
                }
            }
        }
        if (currentAbsence != null)
        {
            aggregatedShifts.Add(currentAbsence);
        }

        aggregatedShifts = aggregatedShifts.OrderBy(s => s.StartDate).ToList();

        int totalCount = aggregatedShifts.Count;
        int pageCount = (int)Math.Ceiling((double)totalCount / pageSize);

        var pagedShifts = aggregatedShifts
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return (pagedShifts, pageCount);
    }

    public async Task<bool> CreateShift(CreateShiftRequest request)
    {
        var endDate = request.EndDate ?? request.StartDate;

        var existingShifts = await _context.EmployeeShifts
            .Where(es => es.EmployeeId == request.EmployeeId
                      && es.ShiftDate >= request.StartDate
                      && es.ShiftDate <= endDate)
            .ToListAsync();

        if (existingShifts.Any()) 
        { 
            return false; 
        }

        if (request.Type == ShiftTypes.Work)
        {
            await _context.EmployeeShifts.AddAsync(new EmployeeShift
            {
                EmployeeId = request.EmployeeId,
                ShiftType = request.Type,
                ShiftDate = request.StartDate,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                OfficeId = request.OfficeId
            });
        }
        else
        {
            for (var date = request.StartDate; date <= endDate; date = date.AddDays(1))
            {
                await _context.EmployeeShifts.AddAsync(new EmployeeShift
                {
                    EmployeeId = request.EmployeeId,
                    ShiftType = request.Type,
                    ShiftDate = date,
                    StartTime = null,
                    EndTime = null,   
                    OfficeId = null   
                });
            }
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteShift(int shiftId)
    {
        // 1. Знаходимо запис, на який клікнув користувач
        var initialShift = await _context.EmployeeShifts.FindAsync(shiftId);

        if (initialShift == null)
        {
            return false;
        }

        if (initialShift.ShiftType == ShiftTypes.Work)
        {
            _context.EmployeeShifts.Remove(initialShift);
            await _context.SaveChangesAsync();
            return true;
        }

        var relatedShifts = await _context.EmployeeShifts
            .Where(es => es.EmployeeId == initialShift.EmployeeId
                      && es.ShiftType == initialShift.ShiftType)
            .OrderBy(es => es.ShiftDate)
            .ToListAsync();

        var shiftsToDelete = new List<EmployeeShift> { initialShift };

        var nextDate = initialShift.ShiftDate.AddDays(1);
        foreach (var s in relatedShifts.Where(s => s.ShiftDate > initialShift.ShiftDate))
        {
            if (s.ShiftDate == nextDate)
            {
                shiftsToDelete.Add(s);
                nextDate = nextDate.AddDays(1);
            }
            else break; // ланцюжок розірвався
        }


        _context.EmployeeShifts.RemoveRange(shiftsToDelete);
        await _context.SaveChangesAsync();

        return true;
    
    }

}
