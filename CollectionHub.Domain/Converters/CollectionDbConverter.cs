﻿using CollectionHub.DataManagement;
using CollectionHub.Models.Enums;
using CollectionHub.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CollectionHub.Domain.Converters
{
    public static class CollectionDbConverter
    {
        public static List<CollectionViewModel> ToCollectionViewModelList(this List<CollectionDb> collectionDb)
        {
            var result = new List<CollectionViewModel>();

            foreach (var item in collectionDb)
            {
                result.Add(new CollectionViewModel
                {
                    Id = item.Id,
                    Name = item.Name,
                    Description = item.Description,
                    ImageUrl = item.ImageUrl
                });
            }

            return result;
        }

        public static Dictionary<string, string> GetNonNullStringFields(this CollectionDb collection)
        {
            var propertyNames = StringConverter.GetCollectionFieldNames();
            var propertyInfos = collection.GetType().GetProperties();

            return propertyInfos
                .Where(property =>
                    propertyNames.Contains(property.Name) &&
                    property.PropertyType == typeof(string) &&
                    property.GetValue(collection) != null)
                .ToDictionary(property => property.Name, property => (string)property.GetValue(collection)!);
        }

        public static CollectionViewModel ToCollectionViewModel(this CollectionDb collectionDb, List<List<string>> items, List<string> categories)
        {
            return new CollectionViewModel
            {
                Id = collectionDb.Id,
                Name = collectionDb.Name,
                Description = collectionDb.Description,
                ItemsDataTypes = GetItemsDataTypes(),
                AllHeaders = GetItemHeaders(collectionDb.GetNonNullStringFields()),
                Items = items, 
                Category = collectionDb.Category.Name,
                ImageUrl = collectionDb.ImageUrl,
                Categories = categories.ToSelectListItem()
            };
        }

        private static List<SelectListItem> GetItemsDataTypes()
        {
            return new List<SelectListItem>
            {
                new SelectListItem { Text = DataType.String.ToString() },
                new SelectListItem { Text = DataType.Integer.ToString() },
                new SelectListItem { Text = DataType.Text.ToString() },
                new SelectListItem { Text = DataType.Bool.ToString() },
                new SelectListItem { Text = DataType.Date.ToString() }
            };
        }

        private static Dictionary<string, string> GetItemHeaders(Dictionary<string, string> headersDb)
        {
            var predefinedHeaders = new Dictionary<string, string>
            {
                { nameof(ItemDb.Name), nameof(ItemDb.Name) },
                { nameof(ItemDb.Tags), nameof(ItemDb.Tags) }
            };

            foreach (var item in headersDb)
            {
                predefinedHeaders.Add(item.Key, item.Value);
            }

            return predefinedHeaders;
        }
    }
}