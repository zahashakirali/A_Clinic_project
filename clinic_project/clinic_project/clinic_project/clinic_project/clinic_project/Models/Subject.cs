using System;
using System.Collections.Generic;

namespace clinic_project.Models;

public partial class Subject
{
    public int SubjectId { get; set; }

    public string SubjectName { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<Lecture> Lectures { get; set; } = new List<Lecture>();

    public virtual ICollection<Practical> Practicals { get; set; } = new List<Practical>();
}
