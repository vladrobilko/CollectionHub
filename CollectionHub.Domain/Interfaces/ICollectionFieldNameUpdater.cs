using CollectionHub.DataManagement;
using CollectionHub.Models.Enums;

namespace CollectionHub.Domain.Interfaces
{
    public interface ICollectionFieldNameUpdater
    {
        bool TryUpdateCollectionFieldName(DataType type, CollectionDb collection, string name);
    }
}
