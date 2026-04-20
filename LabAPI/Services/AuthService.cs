using LabAPI.DTOs;
using LabAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LabAPI.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<AppUser> _userManager;

    private readonly IConfiguration _config;

    public AuthService(UserManager<AppUser> userManager, IConfiguration config)
    {
        _userManager = userManager;
        _config = config;
    }

    public async Task<AppUser?> LoginEmployee(EmployeeLoginRequest request)
    {
        var user = await _userManager.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email);

        if (user != null
            && await _userManager.CheckPasswordAsync(user, request.Password))
        {
            return user;
        }

        return null;
    }

    public async Task<AppUser?> LoginPatient(PatientLoginRequest request)
    {
        var user = await _userManager.Users
            .FirstOrDefaultAsync(u => u.PhoneNumber == request.Phone);

        if (user != null
            && await _userManager.CheckPasswordAsync(user, request.Password))
        {
            return user;
        }

        return null;
    }

    public async Task<(string, string)> GenerateToken(AppUser user)
    {
        var userRole = (await _userManager.GetRolesAsync(user)).Single();
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Role, userRole)
        };

        string jwtKey = _config["Jwt:Key"]
            ?? throw new Exception("JWT Key is missing in secrets");

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

        var signingCredentials = new SigningCredentials(
            securityKey,
            SecurityAlgorithms.HmacSha256Signature);

        SecurityToken securityToken = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddHours(2),
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            signingCredentials: signingCredentials);


        return (new JwtSecurityTokenHandler().WriteToken(securityToken), userRole);
    }
}
