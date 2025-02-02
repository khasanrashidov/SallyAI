using System.ComponentModel.DataAnnotations;

namespace OpenAiSample.WebApi.Models.Account
{
    /// <summary>
    /// LoginViewModel is used for login request
    /// </summary>
    public class LoginViewModel
    {
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
