using System;
using System.Collections.Generic;

namespace LabAPI.Models;

public partial class EmployeeShift
{
    public int Id { get; set; }

    public int EmployeeId { get; set; }

    public int? OfficeId { get; set; }

    public DateOnly ShiftDate { get; set; }

    public TimeOnly? StartTime { get; set; }

    public TimeOnly? EndTime { get; set; }

    public string ShiftType { get; set; } = null!;

    public virtual Employee Employee { get; set; } = null!;

    public virtual Office? Office { get; set; }
}
