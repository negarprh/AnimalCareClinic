using System;
using System.ComponentModel.DataAnnotations;

namespace AnimalCareClinic.ViewModels;

public class AppointmentFormViewModel
{
    public int AppointmentId { get; set; }

    [Display(Name = "Veterinarian")]
    [Range(1, int.MaxValue, ErrorMessage = "Veterinarian is required.")]
    public int VeterinarianId { get; set; }

    [Required(ErrorMessage = "Date is required.")]
    [DataType(DataType.Date)]
    [Display(Name = "Date")]
    public DateTime? AppointmentDate { get; set; }

    [Display(Name = "Schedule Slot")]
    [Range(1, int.MaxValue, ErrorMessage = "Schedule slot is required.")]
    public int ScheduleId { get; set; }

    [Display(Name = "Animal")]
    [Range(1, int.MaxValue, ErrorMessage = "Animal is required.")]
    public int AnimalId { get; set; }

    [Required(ErrorMessage = "Reason is required.")]
    [StringLength(200, MinimumLength = 3, ErrorMessage = "Reason must be between 3 and 200 characters.")]
    public string Reason { get; set; } = string.Empty;
}
