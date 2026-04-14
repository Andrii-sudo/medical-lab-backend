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

    public async Task<AppUser?> LoginEmployee(LoginEmployeeRequest request)
    {
        var identityUser = await _userManager.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email);

        if (identityUser != null
            && await _userManager.CheckPasswordAsync(identityUser, request.Password))
        {
            return identityUser;
        }

        return null;
    }

    public async Task<AppUser?> LoginPatient(LoginPatientRequest request)
    {
        var identityUser = await _userManager.Users
            .FirstOrDefaultAsync(u => u.PhoneNumber == request.Phone);

        if (identityUser != null
            && await _userManager.CheckPasswordAsync(identityUser, request.Password))
        {
            return identityUser;
        }

        return null;
    }

    public async Task<string> GenerateToken(AppUser identityUser)
    {
        var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, identityUser.Id.ToString()),
                new Claim(ClaimTypes.Role, await GetUserRole(identityUser))
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


        return new JwtSecurityTokenHandler().WriteToken(securityToken);
    }

    public async Task<string> GetUserRole(AppUser identityUser)
    {
        return (await _userManager.GetRolesAsync(identityUser)).Single();
    }
}
