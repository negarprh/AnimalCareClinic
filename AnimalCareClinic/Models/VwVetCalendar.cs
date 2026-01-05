using System;
using System.Collections.Generic;

namespace AnimalCareClinic.Models;

public partial class VwVetCalendar
{
    public int VeterinarianId { get; set; }

    public string Veterinarian { get; set; } = null!;

    public int ScheduleId { get; set; }

    public DateOnly Date { get; set; }

    public string TimeSlot { get; set; } = null!;

    public string SlotStatus { get; set; } = null!;

    public int? AppointmentId { get; set; }

    public int? AnimalId { get; set; }

    public string? AppointmentStatus { get; set; }
}
