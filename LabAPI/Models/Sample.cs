using System;
using System.Collections.Generic;

namespace LabAPI.Models;

public partial class Sample
{
    public int Id { get; set; }

    public DateTime? CollectionDate { get; set; }

    public string Status { get; set; } = null!;

    public DateOnly? ExpiryDate { get; set; }

    public int OrderNumber { get; set; }

    public virtual LabOrder OrderNumberNavigation { get; set; } = null!;

    public virtual ICollection<Result> Results { get; set; } = new List<Result>();
}
