using Microsoft.AspNetCore.Identity;

namespace CollectionHub.DataManagement
{
    public partial class UserDb : IdentityUser
    {
        public string ViewName { get; set; }

        public DateTimeOffset RegistrationDate { get; set; }

        public DateTimeOffset LastLoginDate { get; set; }

        public bool IsBlocked { get; set; }

        public bool IsAdmin { get; set; }

        public virtual ICollection<CollectionDb> Collections { get; set; } = new List<CollectionDb>();

        public virtual ICollection<CommentDb> Comments { get; set; } = new List<CommentDb>();

        public virtual ICollection<LikeDb> Likes { get; set; } = new List<LikeDb>();
    }
}
