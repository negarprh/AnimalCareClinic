using System.Collections.Generic;

namespace AnimalCareClinic.Models;

public partial class Owner
{
    public string DisplayName
    {
        get
        {
            var baseName = $"{FirstName} {LastName}".Trim();
            var contacts = new List<string>();

            if (!string.IsNullOrWhiteSpace(PhoneNumber))
            {
                contacts.Add(PhoneNumber);
            }

            if (!string.IsNullOrWhiteSpace(Email))
            {
                contacts.Add(Email);
            }

            if (contacts.Count == 0)
            {
                return baseName;
            }

            return $"{baseName} ({string.Join(" / ", contacts)})";
        }
    }
}
