using System;

namespace BlogManagementSystem.Api.DTOs
{
    public class CommentDetailDto
    {
        public int Id { get; set; }
        public string UserId { get; set; } = null!;
        public string Text { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}
