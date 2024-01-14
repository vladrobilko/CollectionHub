namespace CollectionHub.Models.ViewModels
{
    public class ItemViewModel
    {
        public long Id { get; set; }

        public long CollectionId { get; set; }

        public Dictionary<string, Dictionary<string,string>>? AllHeadersWithValues { get; set; }
    }
}
