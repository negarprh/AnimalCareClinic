namespace AnimalCareClinic.Models;

public partial class Appointment
{
    public string DisplayName
    {
        get
        {
            var dateText = AppointmentDate.ToString("yyyy-MM-dd");
            var timeText = AppointmentTime.ToString("HH:mm");
            var statusText = string.IsNullOrWhiteSpace(Status) ? string.Empty : $" ({Status})";
            var animalText = Animal?.DisplayName ?? $"Animal #{AnimalId}";

            return $"{dateText} {timeText} - {animalText}{statusText}";
        }
    }
}
