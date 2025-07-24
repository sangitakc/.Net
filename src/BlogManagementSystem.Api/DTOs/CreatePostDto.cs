using System.ComponentModel.DataAnnotations;

namespace BlogManagementSystem.Api.DTOs
{
    public class CreatePostDto
    {
        [Required]
        public string Title { get; set; } = null!;

        [Required]
        public string Content { get; set; } = null!;
    }
}
