namespace AnimalCareClinic.Models;

public partial class Veterinarian
{
    public string DisplayName
    {
        get
        {
            var baseName = $"Dr. {FirstName} {LastName}".Trim();

            if (string.IsNullOrWhiteSpace(Speciality))
            {
                return baseName;
            }

            return $"{baseName} ({Speciality})";
        }
    }
}
