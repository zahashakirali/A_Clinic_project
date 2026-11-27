using System;
using System.Collections.Generic;

namespace clinic_project.Models;

public partial class Seminar
{
    public int SeminarId { get; set; }

    public string? SeminarTitle { get; set; }

    public string? SeminarDescription { get; set; }

    public DateOnly? SeminarDate { get; set; }

    public TimeOnly? SeminarTime { get; set; }

    public string? HallNo { get; set; }

    public int? StaffId { get; set; }

    public virtual Staff? Staff { get; set; }
}
