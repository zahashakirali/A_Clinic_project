using System;
using System.Collections.Generic;

namespace clinic_project.Models;

public partial class Lecture
{
    public int LectureId { get; set; }

    public int? SubjectId { get; set; }

    public int? StaffId { get; set; }

    public string? LectureTitle { get; set; }

    public string? LectureDescription { get; set; }

    public DateOnly? LectureDate { get; set; }

    public TimeOnly? LectureTime { get; set; }

    public string? RoomNo { get; set; }

    public virtual Staff? Staff { get; set; }

    public virtual Subject? Subject { get; set; }
}
