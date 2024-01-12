namespace CollectionHub.DataManagement
{
    public partial class LikeDb
    {
        public long Id { get; set; }

        public long ItemId { get; set; }

        public string UserId { get; set; } = null!;

        public virtual ItemDb Item { get; set; } = null!;

        public virtual UserDb User { get; set; } = null!;
    }
}
