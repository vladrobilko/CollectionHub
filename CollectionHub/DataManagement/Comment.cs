namespace CollectionHub.DataManagement
{
    public partial class Comment
    {
        public long Id { get; set; }

        public string UserId { get; set; } = null!;

        public long ItemId { get; set; }

        public string Text { get; set; } = null!;

        public DateTimeOffset CreationDate { get; set; }

        public virtual Item Item { get; set; } = null!;

        public virtual User User { get; set; } = null!;
    }
}
