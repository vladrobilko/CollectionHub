namespace CollectionHub.Domain.Models.ViewModels
{
    public class CommentViewModel
    {
        public string? UserName { get; set; }

        public string? Text { get; set; }

        public DateTimeOffset Date { get; set; }
    }
}
