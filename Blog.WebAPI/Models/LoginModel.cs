using System.ComponentModel.DataAnnotations;

namespace Blog.WebAPI.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "The username field is required.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "The password field is required.")]
        public string Password { get; set; }
    }
}