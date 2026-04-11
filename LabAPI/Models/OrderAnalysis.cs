using System;
using System.Collections.Generic;

namespace LabAPI.Models;

public partial class OrderAnalysis
{
    public int Id { get; set; }

    public int OrderNumber { get; set; }

    public int AnalysisId { get; set; }

    public virtual Analysis Analysis { get; set; } = null!;

    public virtual LabOrder OrderNumberNavigation { get; set; } = null!;
}
