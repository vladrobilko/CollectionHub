namespace CollectionHub.DataManagement
{
    public partial class CategoryDb
    {
        public byte Id { get; set; }

        public string Name { get; set; } = null!;

        public virtual ICollection<CollectionDb> Collections { get; set; } = new List<CollectionDb>();
    }
}
