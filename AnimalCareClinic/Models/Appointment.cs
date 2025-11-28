using System;
using System.Collections.Generic;

namespace AnimalCareClinic.Models;

public partial class Appointment
{
    public int AppointmentId { get; set; }

    public int ScheduleId { get; set; }

    public int AnimalId { get; set; }

    public DateOnly AppointmentDate { get; set; }

    public TimeOnly AppointmentTime { get; set; }

    public string? Reason { get; set; }

    public string Status { get; set; } = null!;

    public virtual Animal Animal { get; set; } = null!;

    public virtual Schedule Schedule { get; set; } = null!;

    public virtual ICollection<VisitHistory> VisitHistories { get; set; } = new List<VisitHistory>();
}
