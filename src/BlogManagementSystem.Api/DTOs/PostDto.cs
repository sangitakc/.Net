using System;
using System.Collections.Generic;

namespace BlogManagementSystem.Api.DTOs
{
    public class PostDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Content { get; set; } = null!;
        public string AuthorId { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public int VoteCount { get; set; }
        public int CommentCount { get; set; }
        public List<CommentDetailDto> Comments { get; set; } = new List<CommentDetailDto>();
    }
}
