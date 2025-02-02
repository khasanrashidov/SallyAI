using System.ComponentModel.DataAnnotations;

namespace OpenAiSample.WebApi.Models.Account
{
    /// <summary>
    /// ChangePasswordViewModel is used for changing user password
    /// </summary>
    public class ChangePasswordViewModel
    {
        [Required]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(
            128,
            MinimumLength = 8,
            ErrorMessage = "Password must be between 8 and 128 characters."
        )]
        public string NewPassword { get; set; }

        [Required]
        public string ConfirmNewPassword { get; set; }
    }
}
