using System;
using System.Collections.Generic;

namespace AnimalCareClinic.Models;

public partial class VwVisitSummary
{
    public int VisitId { get; set; }

    public DateOnly VisitDate { get; set; }

    public int AnimalId { get; set; }

    public string AnimalName { get; set; } = null!;

    public int OwnerId { get; set; }

    public string OwnerName { get; set; } = null!;

    public int VeterinarianId { get; set; }

    public string VetName { get; set; } = null!;

    public string? Diagnosis { get; set; }

    public string? Treatment { get; set; }

    public string? Prescription { get; set; }
}
