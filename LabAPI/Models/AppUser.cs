using Microsoft.AspNetCore.Identity;

namespace LabAPI.Models;
public class AppUser : IdentityUser<int>
{
    public int? EmployeeId { get; set; }
    public virtual Employee? Employee { get; set; }

    public int? PatientId { get; set; }
    public virtual Patient? Patient { get; set; }
}

