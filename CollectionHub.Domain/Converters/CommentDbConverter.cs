using CollectionHub.DataManagement;
using CollectionHub.Domain.Models.ViewModels;

namespace CollectionHub.Domain.Converters
{
    public static class CommentDbConverter
    {
        public static CommentViewModel ToCommentViewModel(this CommentDb commentDb, UserDb user) =>
            new CommentViewModel
            {
                UserName = user.ViewName,
                Text = commentDb.Text,
                Date = commentDb.CreationDate
            };
    }
}
