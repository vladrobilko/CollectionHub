using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace CollectionHub.Models
{
    public class LoginUserViewModel
    {
        [Required(ErrorMessage = "Please enter your email address.")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Please enter a password.")]
        public string? Password { get; set; }

        [BindNever]
        public string? ErrorMessage { get; set; }
    }
}
