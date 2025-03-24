using System.ComponentModel.DataAnnotations;

namespace Blog.WebAPI.Models
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "The username field is required.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "The email field is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "The password field is required.")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters.")]
        public string Password { get; set; }
    }
}