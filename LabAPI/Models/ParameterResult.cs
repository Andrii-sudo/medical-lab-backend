using System;
using System.Collections.Generic;

namespace LabAPI.Models;

public partial class ParameterResult
{
    public int Id { get; set; }

    public decimal? MeasuredValue { get; set; }

    public bool? IsNormal { get; set; }

    public int ResultId { get; set; }

    public int ParameterId { get; set; }

    public virtual Parameter Parameter { get; set; } = null!;

    public virtual Result Result { get; set; } = null!;
}
