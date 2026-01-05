using System;
using System.Collections.Generic;

namespace AnimalCareClinic.Models
{
    public partial class UserAccount
    {
        public int UserId { get; set; }

        public string Username { get; set; } = null!;

        public string Password { get; set; } = null!;

        public string Role { get; set; } = null!;
    }
}
