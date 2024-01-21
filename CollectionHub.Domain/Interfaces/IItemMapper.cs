using CollectionHub.DataManagement;
using Microsoft.AspNetCore.Http;

namespace CollectionHub.Domain.Interfaces
{
    public interface IItemMapper
    {
        Dictionary<string, Dictionary<string, string>> MapCollectionToItemProperties(CollectionDb collection, ItemDb item);

        void MapFormFieldsToItem(ItemDb itemToUpdate, IFormCollection formCollection, bool isItemNew);

        List<List<string>> MapItemValuesToLists(List<ItemDb> items, Dictionary<string, string> fieldNames);
    }
}
