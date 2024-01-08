namespace CollectionHub.DataManagement
{
    public partial class TagDb
    {
        public long Id { get; set; }

        public long ItemId { get; set; }

        public string Name { get; set; } = null!;

        public virtual ItemDb Item { get; set; } = null!;
    }
}
