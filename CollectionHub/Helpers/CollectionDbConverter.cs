using CollectionHub.DataManagement;
using CollectionHub.Models.Enums;
using CollectionHub.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CollectionHub.Helpers
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

        public static CollectionViewModel ToCollectionViewModel(this CollectionDb collectionDb, Dictionary<string, string> headersDb)
        {
            return new CollectionViewModel
            {
                Id = collectionDb.Id,
                Name = collectionDb.Name,
                Description = collectionDb.Description,
                ItemsDataTypes = GetItemsDataTypes(),
                AllHeaders = GetHeaders(headersDb)
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

        private static Dictionary<string, string> GetHeaders(Dictionary<string, string> headersDb)
        {
            var predefinedHeaders = new Dictionary<string, string>
            {
                {  "Name", "Name" },
                { "Tags" ,"Tags"  }
            };
            foreach (var item in headersDb)
                predefinedHeaders.Add(item.Key, item.Value);
            return predefinedHeaders;
        }
    }
}