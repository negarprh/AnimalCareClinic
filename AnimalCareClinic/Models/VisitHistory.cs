using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AnimalCareClinic.Models
{
    public partial class VisitHistory
    {
        public int VisitId { get; set; }

        // FK — always required
        [Required(ErrorMessage = "Appointment is required.")]
        public int AppointmentId { get; set; }

        [Required(ErrorMessage = "Animal is required.")]
        public int AnimalId { get; set; }

        [Required(ErrorMessage = "Veterinarian is required.")]
        public int VeterinarianId { get; set; }

        // Visit date
        [Required(ErrorMessage = "Visit date is required.")]
        [DataType(DataType.Date)]
        public DateOnly VisitDate { get; set; }

        // Diagnosis text
        [Required(ErrorMessage = "Diagnosis is required.")]
        [StringLength(255, MinimumLength = 5,
            ErrorMessage = "Diagnosis must be between 5 and 255 characters.")]
        public string? Diagnosis { get; set; }

        // Treatment given
        [Required(ErrorMessage = "Treatment details are required.")]
        [StringLength(255, MinimumLength = 5,
            ErrorMessage = "Treatment must be between 5 and 255 characters.")]
        public string? Treatment { get; set; }

        // Prescription text (optional but limited)
        [StringLength(255, ErrorMessage = "Prescription cannot exceed 255 characters.")]
        public string? Prescription { get; set; }

        public virtual Animal Animal { get; set; } = null!;

        public virtual Appointment Appointment { get; set; } = null!;

        public virtual Veterinarian Veterinarian { get; set; } = null!;
    }
}
