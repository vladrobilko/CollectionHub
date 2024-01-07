namespace CollectionHub.DataManagement
{
    public partial class Category
    {
        public byte Id { get; set; }

        public string Name { get; set; } = null!;

        public virtual ICollection<Collection> Collections { get; set; } = new List<Collection>();
    }
}
