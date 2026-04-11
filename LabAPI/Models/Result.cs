using System;
using System.Collections.Generic;

namespace LabAPI.Models;

public partial class Result
{
    public int Id { get; set; }

    public DateTime? ResultDate { get; set; }

    public string Status { get; set; } = null!;

    public string? Conclusion { get; set; }

    public int SampleId { get; set; }

    public int AnalysisId { get; set; }

    public virtual Analysis Analysis { get; set; } = null!;

    public virtual ICollection<ParameterResult> ParameterResults { get; set; } = new List<ParameterResult>();

    public virtual Sample Sample { get; set; } = null!;
}
