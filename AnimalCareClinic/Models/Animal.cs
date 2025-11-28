using System;
using System.Collections.Generic;

namespace AnimalCareClinic.Models;

public partial class Animal
{
    public int AnimalId { get; set; }

    public int OwnerId { get; set; }

    public string Name { get; set; } = null!;

    public string Species { get; set; } = null!;

    public int? Age { get; set; }

    public string? Gender { get; set; }

    public string? MedicalHistory { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual Owner Owner { get; set; } = null!;

    public virtual ICollection<VisitHistory> VisitHistories { get; set; } = new List<VisitHistory>();
}
