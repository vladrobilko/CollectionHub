using Microsoft.AspNetCore.Identity;

namespace CollectionHub.Models
{
    public class User : IdentityUser
    {
        public string Id { get; set; }

        public string ViewName { get; set; }

        public DateTime RegistrationDate { get; set; }

        public DateTime LastLoginDate { get; set; }

        public bool IsBlocked { get; set; }

        public bool IsAdmin { get; set; }
    }
}
