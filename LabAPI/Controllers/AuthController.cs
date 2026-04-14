using LabAPI.DTOs;
using LabAPI.Models;
using LabAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LabAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("LoginEmployee")]
    public async Task<IActionResult> LoginEmployee(LoginEmployeeRequest request)
    {
        var identityUser = await _authService.LoginEmployee(request);
        if (identityUser != null)
        {
            string token = await _authService.GenerateToken(identityUser);
            string role = await _authService.GetUserRole(identityUser);

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddHours(2)
            };
            base.Response.Cookies.Append("jwt_token", token, cookieOptions);

            return Ok(new
            {
                id = identityUser.EmployeeId,
                role = role
            });
        }
        return Unauthorized(new { msg = "Неправильна пошта або пароль" });
    }

    [HttpPost("LoginPatient")]
    public async Task<IActionResult> LoginPatient(LoginPatientRequest request)
    {
        var identityUser = await _authService.LoginPatient(request);
        if (identityUser != null)
        {
            string token = await _authService.GenerateToken(identityUser);
            string role = await _authService.GetUserRole(identityUser);

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddHours(2)
            };
            base.Response.Cookies.Append("jwt_token", token, cookieOptions);

            return Ok(new
            {
                id = identityUser.PatientId,
                role = role
            });
        }
        return Unauthorized(new { msg = "Неправильний номер або пароль" });
    }

    [Authorize]
    [HttpPost("Logout")]
    public IActionResult Logout()
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
        };
        base.Response.Cookies.Delete("jwt_token", cookieOptions);

        return Ok();
    }
}
