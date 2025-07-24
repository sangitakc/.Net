namespace BlogManagementSystem.Api.Models
{
    public class Vote
    {
        public int PostId { get; set; }
        public string UserId { get; set; } = null!;
        public bool IsUpvote { get; set; }
    }
}
