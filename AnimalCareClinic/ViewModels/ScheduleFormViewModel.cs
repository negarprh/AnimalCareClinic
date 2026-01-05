using System;
using System.ComponentModel.DataAnnotations;

namespace AnimalCareClinic.ViewModels;

public class ScheduleFormViewModel
{
    public int ScheduleId { get; set; }

    [Display(Name = "Veterinarian")]
    [Range(1, int.MaxValue, ErrorMessage = "Veterinarian is required.")]
    public int VeterinarianId { get; set; }

    [Required(ErrorMessage = "Date is required.")]
    [DataType(DataType.Date)]
    public DateTime? Date { get; set; }

    [Required(ErrorMessage = "Time slot is required.")]
    [Display(Name = "Time Slot")]
    [RegularExpression(@"^\d{2}:\d{2}$", ErrorMessage = "Time slot must be in HH:mm format.")]
    public string TimeSlot { get; set; } = string.Empty;

    [Required(ErrorMessage = "Status is required.")]
    public string Status { get; set; } = string.Empty;
}
