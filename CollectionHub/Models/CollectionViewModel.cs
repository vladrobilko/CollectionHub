using System.ComponentModel.DataAnnotations;

namespace CollectionHub.Models
{
    public class CollectionViewModel
    {
        [Required(ErrorMessage = "Please choose a theme.")]
        public string Theme { get; set; }

        [Required(ErrorMessage = "Please enter a collection name.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please enter a description.")]
        public string Description { get; set; }
    }
}
