using System.ComponentModel.DataAnnotations;

namespace CarCatalog.Models.Requests
{
    public class UpdateProfileRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string? Username { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        [Url]
        public string? AvatarUrl { get; set; }

        [Phone]
        public string? Phone { get; set; }

        [MinLength(6, ErrorMessage = "Пароль должен содержать минимум 6 символов")]
        public string? NewPassword { get; set; }

        [Required]
        public string CurrentPassword { get; set; }
    }
}
