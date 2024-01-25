using CollectionHub.DataManagement;
using CollectionHub.Domain.Converters;
using CollectionHub.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Reflection;

namespace CollectionHub.Domain
{
    public class ItemMapper : IItemMapper
    {
        public Dictionary<string, Dictionary<string, string>> MapCollectionToItemProperties(CollectionDb collection, ItemDb item)
        {
            var nonNullFieldNames = collection.GetNonNullStringFields();

            var itemProperties = nonNullFieldNames.ToDictionary(
                    kvp => kvp.Key.ToItemDbProperty(),
                    kvp => kvp.Value
                );

            return MapItemToDictionary(itemProperties, item);
        }

        public void MapFormFieldsToItem(ItemDb itemToUpdate, IFormCollection formCollection, bool isItemNew)
        {
            var fieldsWithValues = formCollection.ToDictionary();
            var itemDbProperties = typeof(ItemDb).GetProperties();

            foreach (var item in fieldsWithValues)
            {
                if (item.Key == nameof(ItemDb.Tags))
                {
                    itemToUpdate.Tags = item.Value.ToTags();
                }
                else
                {
                    var propertyName = isItemNew ? item.Key.ToItemDbProperty() : item.Key;
                    var property = itemDbProperties.FirstOrDefault(p => p.Name == propertyName);

                    SetNullebleProperty(item, property, itemToUpdate);
                }
            }
        }

        public List<List<string>> MapItemValuesToLists(List<ItemDb> items, Dictionary<string, string> fieldNames)
        {
            var itemProperties = fieldNames.ToItemPropertyNames();
            var result = new List<List<string>>();

            foreach (var item in items)
            {
                var itemValues = new List<string>
                {
                    item.Id.ToString(),
                    item.Name,
                    item.ToTagsString()
                };

                AddPropertyValuesToList(itemValues, itemProperties, item);

                result.Add(itemValues);
            }

            return result;
        }

        private void AddPropertyValuesToList(List<string> itemValues, List<string> itemProperties, ItemDb item)
        {
            foreach (var fieldName in itemProperties)
            {
                var propertyInfo = typeof(ItemDb).GetProperty(fieldName);

                if (propertyInfo != null)
                {
                    var value = GetPropertyValueAsString(propertyInfo, item);
                    itemValues.Add(value);
                }
            }
        }

        private void SetNullebleProperty(KeyValuePair<string, string> item, PropertyInfo property, ItemDb itemToSet)
        {
            if (property != null)
            {
                var value = item.Value.ToDynamicType(property.PropertyType);
                var valueRes = ChangeType(value, property.PropertyType);
                property.SetValue(itemToSet, valueRes);
            }
        }

        private object? ChangeType(object value, Type conversion)
        {
            var t = conversion;

            if (t.IsGenericType && t.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                if (value == null)
                {
                    return null;
                }

                t = Nullable.GetUnderlyingType(t);
            }

            return Convert.ChangeType(value, t);
        }

        private Dictionary<string, Dictionary<string, string>> MapItemToDictionary(Dictionary<string, string> itemProperties, ItemDb item)
        {
            var result = new Dictionary<string, Dictionary<string, string>>
            {
                { nameof(ItemDb.Name), MapItemProperty(nameof(ItemDb.Name), item.Name) },
                { nameof(ItemDb.Tags), MapItemProperty(nameof(ItemDb.Tags), item.ToTagsString()) },
            };

            foreach (var fieldName in itemProperties.Keys)
            {
                var propertyInfo = typeof(ItemDb).GetProperty(fieldName);

                if (propertyInfo != null)
                {
                    var value = GetPropertyValueAsString(propertyInfo, item);
                    result.Add(fieldName, MapItemProperty(itemProperties[fieldName], value));
                }
            }

            return result;
        }

        private string GetPropertyValueAsString(PropertyInfo propertyInfo, ItemDb item)
        {
            var value = propertyInfo.GetValue(item);

            if (propertyInfo.PropertyType == typeof(DateTimeOffset?) && value is DateTimeOffset dateTimeOffset)
            {
                return dateTimeOffset.Date.ToString("yyyy-MM-dd");
            }

            return value?.ToString() ?? "-";
        }

        private Dictionary<string, string> MapItemProperty(string propertyName, string propertyValue)
        {
            return new Dictionary<string, string>
            {
                { propertyName, propertyValue }
            };
        }
    }
}
