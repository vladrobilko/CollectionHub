namespace CollectionHub.DataManagement
{
    public partial class CommentDb
    {
        public long Id { get; set; }

        public string UserId { get; set; } = null!;

        public long ItemId { get; set; }

        public string Text { get; set; } = null!;

        public DateTimeOffset CreationDate { get; set; }

        public virtual ItemDb Item { get; set; } = null!;

        public virtual UserDb User { get; set; } = null!;
    }
}
