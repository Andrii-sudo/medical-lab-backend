using LabAPI.DTOs;
using LabAPI.Models;
using LabAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace LabAPI.Controllers
{
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
                var tokenString = await _authService.GenerateTokenString(identityUser);
                return Ok(new
                {
                    token = tokenString,
                    id = identityUser.EmployeeId,
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
                var tokenString = await _authService.GenerateTokenString(identityUser);
                return Ok(new
                {
                    token = tokenString,
                    id = identityUser.PatientId,
                });
            }
            return Unauthorized(new { msg = "Неправильний номер або пароль" });
        }
    }
}
