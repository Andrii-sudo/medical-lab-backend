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

    [HttpPost("employee/login")]
    public async Task<IActionResult> LoginEmployee(EmployeeLoginRequest request)
    {
        var user = await _authService.LoginEmployee(request);
        if (user != null)
        {
            var (token, role) = await _authService.GenerateToken(user);
            SetTokenCookie(token);

            return Ok(new
            {
                id = user.EmployeeId,
                role = role
            });
        }
        return Unauthorized(new { msg = "Неправильна пошта або пароль" });
    }

    [HttpPost("patient/login")]
    public async Task<IActionResult> LoginPatient(PatientLoginRequest request)
    {
        var user = await _authService.LoginPatient(request);
        if (user != null)
        {
            var (token, role) = await _authService.GenerateToken(user);
            SetTokenCookie(token);

            return Ok(new
            {
                id = user.PatientId,
                role = role
            });
        }
        return Unauthorized(new { msg = "Неправильний номер або пароль" });
    }

    [Authorize]
    [HttpPost("logout")]
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

    private void SetTokenCookie(string token)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTime.UtcNow.AddHours(2)
        };
        base.Response.Cookies.Append("jwt_token", token, cookieOptions);
    }
}
