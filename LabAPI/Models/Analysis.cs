using System;
using System.Collections.Generic;

namespace LabAPI.Models;

public partial class Analysis
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string SampleType { get; set; } = null!;

    public byte ExpiryDays { get; set; }

    public decimal Price { get; set; }

    public virtual ICollection<OrderAnalysis> OrderAnalyses { get; set; } = new List<OrderAnalysis>();

    public virtual ICollection<Parameter> Parameters { get; set; } = new List<Parameter>();

    public virtual ICollection<Result> Results { get; set; } = new List<Result>();
}
