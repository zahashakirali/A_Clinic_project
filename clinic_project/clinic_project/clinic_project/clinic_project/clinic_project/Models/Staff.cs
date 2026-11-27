using System;
using System.Collections.Generic;

namespace clinic_project.Models;

public partial class Staff
{
    public int StaffId { get; set; }

    public string? FullName { get; set; }

    public string? Qualification { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public int? RoleId { get; set; }

    public virtual ICollection<Lecture> Lectures { get; set; } = new List<Lecture>();

    public virtual ICollection<Practical> Practicals { get; set; } = new List<Practical>();

    public virtual Role? Role { get; set; }

    public virtual ICollection<Seminar> Seminars { get; set; } = new List<Seminar>();
}
