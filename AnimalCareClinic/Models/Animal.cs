using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AnimalCareClinic.Models;

public partial class Animal
{
    public int AnimalId { get; set; }

    [Required(ErrorMessage = "Owner is required.")]
    public int OwnerId { get; set; }

    [Required(ErrorMessage = "Name is required.")]
    [StringLength(50, MinimumLength = 2,
        ErrorMessage = "Name must be between 2 and 50 characters.")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "Species is required.")]
    [StringLength(30, MinimumLength = 2,
        ErrorMessage = "Species must be between 2 and 30 characters.")]
    public string Species { get; set; } = null!;

    [Range(0, 40, ErrorMessage = "Age must be between 0 and 40.")]
    public int? Age { get; set; }

    [Required(ErrorMessage = "Gender is required.")]
    [RegularExpression("^(M|F)$",
        ErrorMessage = "Gender must be 'M' or 'F'.")]
    public string Gender { get; set; } = null!;

    [StringLength(200,
        ErrorMessage = "Medical history must be at most 200 characters.")]
    public string? MedicalHistory { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual Owner Owner { get; set; } = null!;

    public virtual ICollection<VisitHistory> VisitHistories { get; set; } = new List<VisitHistory>();
}


