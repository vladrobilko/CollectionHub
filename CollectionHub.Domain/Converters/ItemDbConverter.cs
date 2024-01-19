using CollectionHub.DataManagement;
using CollectionHub.Domain.Models;
using CollectionHub.Domain.Models.ViewModels;
using CollectionHub.Models.ViewModels;

namespace CollectionHub.Domain.Converters
{
    public static class ItemDbConverter
    {
        public static List<ItemViewModel> ToItemViewModelList(this List<ItemDb> itemsDb)
        {
            var itemsViewList = new List<ItemViewModel>();

            foreach (var item in itemsDb)
            {
                itemsViewList.Add(new ItemViewModel
                {
                    Id = item.Id,
                    CollectionId = item.CollectionId,
                    Name = item.Name,
                    CollectionName = item.Collection.Name,
                    AuthorName = item.Collection.User.ViewName
                });
            }

            return itemsViewList;
        }

        public static List<CommentViewModel> ToCommentViewModelList(this ItemDb item)
        {
            var comments = new List<CommentViewModel>();

            foreach (var comment in item.Comments)
            {
                comments.Add(new CommentViewModel
                {
                    UserName = comment.User.ViewName,
                    Text = comment.Text,
                    Date = comment.CreationDate
                });
            }

            return comments.OrderBy(x => x.Date).ToList();
        }

        public static ItemAlgoliaModel ToItemAlgoliaModel(this ItemDb item)
        {
            return new ItemAlgoliaModel
            {
                ObjectID = item.Id.ToString(),
                CollectionId = item.CollectionId,
                Name = item.Name,
                CollectionName = item.Collection.Name,
                Tags = string.Join(", ", item.Tags.Select(tag => tag.Name)),
                Text = $"{item.String1Value ?? ""} {item.String2Value ?? ""} {item.String3Value ?? ""} {item.Text1Value ?? ""} {item.Text2Value ?? ""} {item.Text3Value ?? ""}"
            };
        }
    }
}
