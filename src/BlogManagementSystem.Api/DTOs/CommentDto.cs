using System.ComponentModel.DataAnnotations;

namespace BlogManagementSystem.Api.DTOs
{
    public class CommentDto
    {
        [Required]
        public int PostId { get; set; }
        [Required]
        public string Text { get; set; } = null!;
    }
}
