using Microsoft.AspNetCore.Identity;

namespace CollectionHub.DataManagement
{
    public partial class User : IdentityUser
    {
        public string Id { get; set; }

        public string ViewName { get; set; }

        public DateTime RegistrationDate { get; set; }

        public DateTime LastLoginDate { get; set; }

        public bool IsBlocked { get; set; }

        public bool IsAdmin { get; set; }

        public virtual ICollection<Collection> Collections { get; set; } = new List<Collection>();

        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

        public virtual ICollection<Like> Likes { get; set; } = new List<Like>();
    }
}
