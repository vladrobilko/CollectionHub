using CollectionHub.Domain.Models.ViewModels;

namespace CollectionHub.Models.ViewModels
{
    public class ItemViewModel
    {
        public long Id { get; set; }

        public long CollectionId { get; set; }

        public string? Name { get; set; }

        public string? CollectionName { get; set; }

        public string? AuthorName { get; set; }

        public long? Likes { get; set; }

        public Dictionary<string, Dictionary<string, string>>? AllHeadersWithValues { get; set; }

        public List<CommentViewModel> Comments { get; set; }
    }
}
