using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AnimalCareClinic.Models;

public partial class Schedule
{
    public int ScheduleId { get; set; }

    [Required(ErrorMessage = "Veterinarian is required.")]
    [Display(Name = "Veterinarian")]
    public int VeterinarianId { get; set; }

    [Required(ErrorMessage = "Date is required.")]
    [DataType(DataType.Date)]
    [Display(Name = "Date")]
    public DateOnly Date { get; set; }

    [Required(ErrorMessage = "Time Slot is required.")]
    [Display(Name = "Time Slot")]
    [RegularExpression(@"^\d{2}:\d{2}$", ErrorMessage = "Time slot must be in HH:mm format.")]
    public string TimeSlot { get; set; } = null!;

    [Required(ErrorMessage = "Status is required.")]
    public string Status { get; set; } = null!;

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    public virtual Veterinarian Veterinarian { get; set; } = null!;
}
