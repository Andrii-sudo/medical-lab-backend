using System;
using System.Collections.Generic;

namespace LabAPI.Models;

public partial class Office
{
    public int Id { get; set; }

    public short Number { get; set; }

    public string City { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string Type { get; set; } = null!;

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual ICollection<EmployeeSchedule> EmployeeSchedules { get; set; } = new List<EmployeeSchedule>();

    public virtual ICollection<EmployeeShift> EmployeeShifts { get; set; } = new List<EmployeeShift>();

    public virtual ICollection<OfficeSchedule> OfficeSchedules { get; set; } = new List<OfficeSchedule>();
}
