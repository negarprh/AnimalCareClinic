using System;
using System.Collections.Generic;

namespace AnimalCareClinic.Models;

public partial class Schedule
{
    public int ScheduleId { get; set; }

    public int VeterinarianId { get; set; }

    public DateOnly Date { get; set; }

    public string TimeSlot { get; set; } = null!;

    public string Status { get; set; } = null!;

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual Veterinarian Veterinarian { get; set; } = null!;
}
