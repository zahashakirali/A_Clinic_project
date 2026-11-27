using System;
using System.Collections.Generic;

namespace clinic_project.Models;

public partial class Equipment
{
    public int EquipmentId { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public decimal? Price { get; set; }

    public int? Stock { get; set; }

    public string? Image { get; set; }
}
