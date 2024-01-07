using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace CollectionHub.Models.ViewModels
{
    public class CollectionViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Please choose a theme.")]
        public string? Theme { get; set; }

        [Required(ErrorMessage = "Please enter a collection name.")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Please enter a description.")]
        public string? Description { get; set; }

        public string? ImageLink { get; set; }

        public IFormFile? File { get; set; }

        public List<SelectListItem>? Themes { get; set; }

        public List<ItemViewModel> Items { get; set; }

        public List<SelectListItem> ItemsDataTypes { get; set; }

        public List<string> AllHeaders { get; set; }
    }
}
