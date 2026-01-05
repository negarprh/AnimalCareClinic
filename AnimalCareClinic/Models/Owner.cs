using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;



namespace AnimalCareClinic.Models;

public partial class Owner
{
    public int OwnerId { get; set; }

    [Required]
    [StringLength(50, MinimumLength = 2,
        ErrorMessage = "First name must be between 2 and 50 characters.")]
    public string FirstName { get; set; } = null!;

    [Required]
    [StringLength(50, MinimumLength = 2,
        ErrorMessage = "Last name must be between 2 and 50 characters.")]
    public string LastName { get; set; } = null!;

    [Required]
    [StringLength(100, ErrorMessage = "Address must be at most 100 characters.")]
    public string Address { get; set; } = null!;

    [Required]
    [Phone(ErrorMessage = "Phone number format is not valid.")]
    public string PhoneNumber { get; set; } = null!;

    [Required]
    [EmailAddress(ErrorMessage = "Email format is not valid.")]
    public string Email { get; set; } = null!;

    public virtual ICollection<Animal> Animals { get; set; } = new List<Animal>();
}



