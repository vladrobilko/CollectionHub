using CollectionHub.DataManagement;
using CollectionHub.Domain.Converters;
using CollectionHub.Domain.Interfaces;
using CollectionHub.Models.Enums;

namespace CollectionHub.Domain
{
    public class CollectionFieldNameUpdater : ICollectionFieldNameUpdater
    {
        public bool UpdateCollectionFieldName(DataType type, CollectionDb collection, string name)
        {
            var propertyNames = type.ToCollectionProperty();

            if (IsFieldExist(propertyNames, collection, name))
            {
                return false;
            }

            foreach (var propertyName in propertyNames)
            {
                var property = collection.GetType().GetProperty(propertyName);
                var value = (string)property.GetValue(collection);

                if (value == null)
                {
                    property.SetValue(collection, name);
                    return true;
                }
            }

            return false;
        }

        private bool IsFieldExist(string[] propertyNames, CollectionDb collection, string name)
        {
            return propertyNames
                .Select(propertyName => (string)collection.GetType().GetProperty(propertyName).GetValue(collection))
                .ToList().Contains(name);
        }
    }
}
