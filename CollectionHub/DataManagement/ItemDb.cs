namespace CollectionHub.DataManagement
{
    public partial class ItemDb
    {
        public long Id { get; set; }

        public long CollectionId { get; set; }

        public string Name { get; set; } = null!;

        public DateTimeOffset CreationDate { get; set; }

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

        public DateTimeOffset? Date1Value { get; set; }

        public DateTimeOffset? Date2Value { get; set; }

        public DateTimeOffset? Date3Value { get; set; }

        public virtual CollectionDb Collection { get; set; } = null!;

        public virtual ICollection<CommentDb> Comments { get; set; } = new List<CommentDb>();

        public virtual ICollection<LikeDb> Likes { get; set; } = new List<LikeDb>();

        public virtual ICollection<TagDb> Tags { get; set; } = new List<TagDb>();
    }
}
