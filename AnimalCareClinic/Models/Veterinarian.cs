using System;
using System.Collections.Generic;

namespace AnimalCareClinic.Models;

public partial class Veterinarian
{
    public int VeterinarianId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? Speciality { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Email { get; set; }

    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();

    public virtual ICollection<VisitHistory> VisitHistories { get; set; } = new List<VisitHistory>();
}
