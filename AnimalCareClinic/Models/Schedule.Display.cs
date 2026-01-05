namespace AnimalCareClinic.Models;

public partial class Schedule
{
    public string DisplayName
    {
        get
        {
            var dateText = Date.ToString("yyyy-MM-dd");
            var timeText = string.IsNullOrWhiteSpace(TimeSlot) ? "00:00" : TimeSlot;
            var statusText = string.IsNullOrWhiteSpace(Status) ? string.Empty : $" ({Status})";
            var vetText = Veterinarian?.DisplayName ?? $"Vet #{VeterinarianId}";

            return $"{dateText} {timeText}{statusText} - {vetText}";
        }
    }
}
