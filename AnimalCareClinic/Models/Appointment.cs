using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AnimalCareClinic.Models;

public partial class Appointment
{
    public int AppointmentId { get; set; }

    [Required(ErrorMessage = "Schedule is required.")]
    [Display(Name = "Schedule")]
    public int ScheduleId { get; set; }

    [Required(ErrorMessage = "Animal is required.")]
    [Display(Name = "Animal")]
    public int AnimalId { get; set; }

    [Required(ErrorMessage = "Appointment date is required.")]
    [DataType(DataType.Date)]
    [Display(Name = "Appointment Date")]
    public DateOnly AppointmentDate { get; set; }

    [Required(ErrorMessage = "Appointment time is required.")]
    [DataType(DataType.Time)]
    [Display(Name = "Appointment Time")]
    public TimeOnly AppointmentTime { get; set; }

    [Required(ErrorMessage = "Reason is required.")]
    [StringLength(200, MinimumLength = 3, ErrorMessage = "Reason must be between 3 and 200 characters.")]
    public string? Reason { get; set; }

    [Required(ErrorMessage = "Status is required.")]
    [Display(Name = "Appointment Status")]
    public string Status { get; set; } = null!;

    public virtual Animal Animal { get; set; } = null!;
    public virtual Schedule Schedule { get; set; } = null!;
    public virtual ICollection<VisitHistory> VisitHistories { get; set; } = new List<VisitHistory>();
}
