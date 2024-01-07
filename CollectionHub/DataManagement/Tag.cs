namespace CollectionHub.DataManagement
{
    public partial class Tag
    {
        public long Id { get; set; }

        public long ItemId { get; set; }

        public string Name { get; set; } = null!;

        public virtual Item Item { get; set; } = null!;
    }
}
