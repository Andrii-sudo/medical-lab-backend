using System;
using System.Collections.Generic;

namespace LabAPI.Models;

public partial class LabOrder
{
    public int Number { get; set; }

    public DateTime CreatedDate { get; set; }

    public string Status { get; set; } = null!;

    public int TotalPrice { get; set; }

    public int PatientId { get; set; }

    public virtual ICollection<OrderAnalysis> OrderAnalyses { get; set; } = new List<OrderAnalysis>();

    public virtual Patient Patient { get; set; } = null!;

    public virtual ICollection<Sample> Samples { get; set; } = new List<Sample>();

    public int OfficeId { get; set; }
    public virtual Office Office { get; set; } = null!;
}
