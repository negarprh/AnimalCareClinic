namespace AnimalCareClinic.Models;

public partial class Animal
{
    public string DisplayName
    {
        get
        {
            var baseName = $"{Name} ({Species})";
            var ownerName = Owner?.DisplayName;

            if (string.IsNullOrWhiteSpace(ownerName))
            {
                return baseName;
            }

            return $"{baseName} - {ownerName}";
        }
    }
}
