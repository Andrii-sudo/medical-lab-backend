using System;
using System.Collections.Generic;

namespace LabAPI.Models;

public partial class ParameterNorm
{
    public int Id { get; set; }

    public byte AgeMin { get; set; }

    public byte AgeMax { get; set; }

    public string Gender { get; set; } = null!;

    public decimal? MinValue { get; set; }

    public decimal? MaxValue { get; set; }

    public int ParameterId { get; set; }

    public virtual Parameter Parameter { get; set; } = null!;
}
