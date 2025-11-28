using System;
using System.Collections.Generic;

namespace AnimalCareClinic.Models;

public partial class Owner
{
    public int OwnerId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? Address { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Email { get; set; }

    public virtual ICollection<Animal> Animals { get; set; } = new List<Animal>();
}
