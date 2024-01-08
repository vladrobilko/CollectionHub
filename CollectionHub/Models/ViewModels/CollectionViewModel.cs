using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace CollectionHub.Models.ViewModels
{
    public class CollectionViewModel
    {
        public long Id { get; set; }

        [Required(ErrorMessage = "Please choose a category.")]
        public string? Category { get; set; }

        [Required(ErrorMessage = "Please enter a collection name.")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Please enter a description.")]
        public string? Description { get; set; }

        public string? ImageUrl { get; set; }

        public IFormFile? File { get; set; }

        public List<SelectListItem>? Categories { get; set; }

        public List<ItemViewModel>? Items { get; set; }

        public List<SelectListItem>? ItemsDataTypes { get; set; }

        public List<string>? AllHeaders { get; set; }
    }
}
