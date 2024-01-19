namespace CollectionHub.Domain.Models
{
    public class ItemAlgoliaModel
    {
        public string ObjectID { get; set; }

        public long CollectionId { get; set; }

        public string Name { get; set; }

        public string CollectionName { get; set; }

        public string Tags { get; set; }

        public string Text { get; set; }
    }
}
