using System;
using System.Collections.Generic;

namespace LabAPI.Models;

public partial class Parameter
{
    public int Id { get; set; }

    public string ParameterName { get; set; } = null!;

    public string? Unit { get; set; }

    public int AnalysisId { get; set; }

    public virtual Analysis Analysis { get; set; } = null!;

    public virtual ICollection<ParameterNorm> ParameterNorms { get; set; } = new List<ParameterNorm>();

    public virtual ICollection<ParameterResult> ParameterResults { get; set; } = new List<ParameterResult>();
}
