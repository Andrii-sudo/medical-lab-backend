using LabAPI.Constants;
using LabAPI.DTOs;
using LabAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LabAPI.Services;
public class EmployeeService: IEmployeeService
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
}
