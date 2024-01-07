using Azure;

namespace CollectionHub.DataManagement
{

    public partial class Item
    {
        public long Id { get; set; }

        public long CollectionId { get; set; }

        public string Name { get; set; } = null!;

        public long CreationDate { get; set; }

        public string? String1Value { get; set; }

        public string? String2Value { get; set; }

        public string? String3Value { get; set; }

        public long? Int1Value { get; set; }

        public long? Int2Value { get; set; }

        public long? Int3Value { get; set; }

        public string? Text1Value { get; set; }

        public string? Text2Value { get; set; }

        public string? Text3Value { get; set; }

        public bool? Bool1Value { get; set; }

        public bool? Bool2Value { get; set; }

        public bool? Bool3Value { get; set; }

        public DateTime? Date1Value { get; set; }

        public DateTime? Date2Value { get; set; }

        public DateTime? Date3Value { get; set; }

        public virtual Collection Collection { get; set; } = null!;

        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

        public virtual ICollection<Like> Likes { get; set; } = new List<Like>();
        public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();
    }

}
