using System.ComponentModel.DataAnnotations;

namespace BlogManagementSystem.Api.DTOs
{
    public class EditPostDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = null!;

        [Required]
        public string Content { get; set; } = null!;
    }
}
