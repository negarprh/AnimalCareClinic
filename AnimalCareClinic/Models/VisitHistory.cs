using System;
using System.Collections.Generic;

namespace AnimalCareClinic.Models;

public partial class VisitHistory
{
    public int VisitId { get; set; }

    public int AppointmentId { get; set; }

    public int AnimalId { get; set; }

    public int VeterinarianId { get; set; }

    public DateOnly VisitDate { get; set; }

    public string? Diagnosis { get; set; }

    public string? Treatment { get; set; }

    public string? Prescription { get; set; }

    public virtual Animal Animal { get; set; } = null!;

    public virtual Appointment Appointment { get; set; } = null!;

    public virtual Veterinarian Veterinarian { get; set; } = null!;
}
