using Microsoft.AspNetCore.Mvc.Rendering;

namespace CollectionHub.Helpers
{
    public static class ListStringConverter
    {
        public static List<SelectListItem> ToSelectListItem(this List<string> list) =>
            list.Select(item => new SelectListItem
            {
                Text = item
            }).ToList();
    }
}
