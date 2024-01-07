namespace CollectionHub.DataManagement
{
    public partial class Like
    {
        public long Id { get; set; }

        public long ItemId { get; set; }

        public string UserId { get; set; } = null!;

        public virtual Item Item { get; set; } = null!;

        public virtual User User { get; set; } = null!;
    }
}
