using CollectionHub.DataManagement;
using CollectionHub.Domain.Models;
using CollectionHub.Models.ViewModels;

namespace CollectionHub.Domain.Converters
{
    public static class ItemAlgoliaModelConverter
    {
        public static ItemAlgoliaModel SetItemDb(this ItemAlgoliaModel itemAlgolia, ItemDb itemDb)
        {
            itemAlgolia.Name = itemDb.Name;
            itemAlgolia.Tags = string.Join(", ", itemDb.Tags.Select(tag => tag.Name));
            itemAlgolia.Text = $"{itemDb.String1Value ?? ""} {itemDb.String2Value ?? ""} {itemDb.String3Value ?? ""} " +
                               $"{itemDb.Text1Value ?? ""} {itemDb.Text2Value ?? ""} {itemDb.Text3Value ?? ""}";

            return itemAlgolia;
        }

        public static ItemViewModel ToItemViewModel(this ItemAlgoliaModel item)
        {
            return new ItemViewModel
            {
                Id = long.Parse(item.ObjectID),
                CollectionId = item.CollectionId,
                Name = item.Name,
                CollectionName = item.CollectionName
            };
        }
    }
}
