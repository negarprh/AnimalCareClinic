using System.Collections.Generic;
using AnimalCareClinic.Models;

namespace AnimalCareClinic.ViewModels;

public class HomeDashboardViewModel
{
    public int TotalAnimals { get; set; }
    public int TotalOwners { get; set; }
    public int TotalAppointments { get; set; }
    public int AvailableSlots { get; set; }
    public List<Appointment> TodaysAppointments { get; set; } = [];
}
