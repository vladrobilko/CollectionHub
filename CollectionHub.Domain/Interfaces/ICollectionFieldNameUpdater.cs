using CollectionHub.DataManagement;
using CollectionHub.Models.Enums;

namespace CollectionHub.Domain.Interfaces
{
    public interface ICollectionFieldNameUpdater
    {
        bool UpdateCollectionFieldName(DataType type, CollectionDb collection, string name);
    }
}
