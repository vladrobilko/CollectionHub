using CollectionHub.DataManagement;
using CollectionHub.Domain.Converters;
using CollectionHub.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Reflection;

namespace CollectionHub.Domain
{
    public class ItemMapper : IItemMapper
    {
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
                    string.Join(", ", item.Tags.Select(tag => "#" + tag.Name))
                };

                foreach (var fieldName in itemProperties)
                {
                    var propertyInfo = typeof(ItemDb).GetProperty(fieldName);

                    if (propertyInfo != null)
                    {
                        var value = GetPropertyValueAsString(propertyInfo, item);
                        itemValues.Add(value);
                    }
                }

                result.Add(itemValues);
            }

            return result;
        }

        public void UpdateItemFromFormFields(ItemDb itemToUpdate, IFormCollection formCollection)
        {
            var fieldsWithValues = formCollection.ToDictionary();

            var itemDbProperties = typeof(ItemDb).GetProperties();

            foreach (var item in fieldsWithValues)
            {
                var property = itemDbProperties.FirstOrDefault(p => p.Name == item.Key);

                if (item.Key == nameof(ItemDb.Tags))
                {
                    itemToUpdate.Tags = item.Value.ToTags();
                }
                else
                {
                    SetNullebleProperty(item, property, itemToUpdate);
                }
            }
        }

        public ItemDb MapFormFieldsToNewItem(IFormCollection formCollection)
        {
            var fieldsWithValues = formCollection.ToDictionary();

            var itemDbProperties = typeof(ItemDb).GetProperties();

            var newItem = new ItemDb();

            foreach (var item in fieldsWithValues)
            {
                var property = itemDbProperties.FirstOrDefault(p => p.Name == item.Key.ToItemDbProperty());

                if (item.Key == nameof(ItemDb.Tags))
                {
                    newItem.Tags = item.Value.ToTags();
                }
                else
                {
                    SetNullebleProperty(item, property, newItem);
                }
            }

            return newItem;
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

        private object ChangeType(object value, Type conversion)
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

        public Dictionary<string, Dictionary<string, string>> MapCollectionToItemProperties(CollectionDb collection, ItemDb item)
        {

            var nonNullFieldNames = collection.GetNonNullStringFields();
            var itemProperties = nonNullFieldNames.ToDictionary(
                    kvp => kvp.Key.ToItemDbProperty(),
                    kvp => kvp.Value
                );

            return MapItemToDictionary(itemProperties, item);
        }

        private Dictionary<string, Dictionary<string, string>> MapItemToDictionary(Dictionary<string, string> itemProperties, ItemDb item)
        {
            var result = new Dictionary<string, Dictionary<string, string>>
            {
                {
                    nameof(ItemDb.Name), new Dictionary<string, string>
                    {
                        { nameof(ItemDb.Name), item.Name }
                    }
                },
                {
                    nameof(ItemDb.Tags), new Dictionary<string, string>
                    {
                        { nameof(ItemDb.Tags), string.Join(", ", item.Tags.Select(tag => "#" + tag.Name)) }
                    }
                },
            };

            foreach (var fieldName in itemProperties.Keys)
            {
                var propertyInfo = typeof(ItemDb).GetProperty(fieldName);

                if (propertyInfo != null)
                {
                    var value = GetPropertyValueAsString(propertyInfo, item);
                    result.Add(fieldName, new Dictionary<string, string>
                    {
                        { itemProperties[fieldName], value }
                    });
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
    }
}
