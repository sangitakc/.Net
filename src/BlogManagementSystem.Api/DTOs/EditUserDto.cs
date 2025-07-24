using System.ComponentModel.DataAnnotations;

namespace BlogManagementSystem.Api.DTOs
{
    public class EditUserDto
    {
        [Required, StringLength(50)]
        public string FirstName { get; set; } = null!;

        [Required, StringLength(50)]
        public string LastName { get; set; } = null!;

        [Required, EmailAddress]
        public string Email { get; set; } = null!;

    }
}
