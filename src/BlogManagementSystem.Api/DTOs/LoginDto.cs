using System.ComponentModel.DataAnnotations;

namespace BlogManagementSystem.Api.DTOs
{
    public class LoginDto
    {
        [Required, StringLength(30,MinimumLength = 3)]
        public string Username { get; set; } = null!;
        [Required]
        public string Password { get; set; } = null!;
    }
}
