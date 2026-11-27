using System;
using System.Collections.Generic;

namespace clinic_project.Models;

public partial class Medicine
{
    public int MedicineId { get; set; }

    public string? Name { get; set; }

    public string? Type { get; set; }

    public string? Code { get; set; }

    public decimal? Price { get; set; }

    public int? Stock { get; set; }

    public string? Image { get; set; }
}
