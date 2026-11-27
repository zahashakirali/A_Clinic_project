using System;
using System.Collections.Generic;

namespace clinic_project.Models;

public partial class Practical
{
    public int PracticalId { get; set; }

    public int? SubjectId { get; set; }

    public int? StaffId { get; set; }

    public string? PracticalTitle { get; set; }

    public string? PracticalDescription { get; set; }

    public DateOnly? PracticalDate { get; set; }

    public TimeOnly? PracticalTime { get; set; }

    public string? RoomNo { get; set; }

    public virtual Staff? Staff { get; set; }

    public virtual Subject? Subject { get; set; }
}
