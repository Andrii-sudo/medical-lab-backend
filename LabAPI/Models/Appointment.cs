using System;
using System.Collections.Generic;

namespace LabAPI.Models;

public partial class Appointment
{
    public int Id { get; set; }

    public int PatientId { get; set; }

    public int OfficeId { get; set; }

    public DateOnly VisitDate { get; set; }

    public TimeOnly VisitTime { get; set; }

    public string Purpose { get; set; } = null!;

    public string Status { get; set; } = null!;

    public virtual Office Office { get; set; } = null!;

    public virtual Patient Patient { get; set; } = null!;
}
