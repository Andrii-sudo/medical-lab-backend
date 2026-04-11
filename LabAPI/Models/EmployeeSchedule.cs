using System;
using System.Collections.Generic;

namespace LabAPI.Models;

public partial class EmployeeSchedule
{
    public int Id { get; set; }

    public int EmployeeId { get; set; }

    public int OfficeId { get; set; }

    public byte DayOfWeek { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public virtual Employee Employee { get; set; } = null!;

    public virtual Office Office { get; set; } = null!;
}
