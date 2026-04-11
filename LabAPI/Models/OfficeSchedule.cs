using System;
using System.Collections.Generic;

namespace LabAPI.Models;

public partial class OfficeSchedule
{
    public int Id { get; set; }

    public int OfficeId { get; set; }

    public byte DayOfWeek { get; set; }

    public TimeOnly OpenTime { get; set; }

    public TimeOnly CloseTime { get; set; }

    public virtual Office Office { get; set; } = null!;
}
