using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogManagementSystem.Api.DTOs;
using BlogManagementSystem.Api.Models;
using BlogManagementSystem.Api.Repositories;
using Microsoft.Extensions.Logging;

namespace BlogManagementSystem.Api.Services
{
    public class PostService : IPostService
    {
        private readonly IPostRepository _repo;
        private readonly ILogger<PostService> _logger;

        public PostService(IPostRepository repo, ILogger<PostService> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        public async Task<BaseResponse<IEnumerable<PostDto>>> GetAllPostsAsync()
        {
            _logger.LogInformation("Fetching all posts");
            var posts = await _repo.GetAllAsync();
            return new BaseResponse<IEnumerable<PostDto>>
            {
                Success = true,
                Message = "Posts retrieved successfully.",
                Data = posts.Select(Map).ToList()
            };
        }

        public async Task<BaseResponse<PostDto>> GetPostByIdAsync(int postId)
        {
            _logger.LogInformation("Fetching post {PostId}", postId);
            var p = await _repo.GetByIdAsync(postId);
            if (p == null)
            {
                _logger.LogWarning("Post {PostId} not found", postId);
                return new BaseResponse<PostDto> { Success = false, Message = "Post not found." };
            }

            return new BaseResponse<PostDto>
            {
                Success = true,
                Message = "Post retrieved successfully.",
                Data = Map(p)
            };
        }

        public async Task<BaseResponse<PostDto>> CreatePostAsync(string userId, CreatePostDto dto)
        {
            _logger.LogInformation("Creating post for UserId={UserId}", userId);
            var post = new Post { Title = dto.Title, Content = dto.Content, AuthorId = userId };
            await _repo.AddAsync(post);

            try
            {
                await _repo.SaveChangesAsync();
                _logger.LogInformation("Created post {PostId}", post.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating post for UserId={UserId}", userId);
                return new BaseResponse<PostDto> { Success = false, Message = $"Error creating post: {ex.Message}" };
            }

            return new BaseResponse<PostDto>
            {
                Success = true,
                Message = "Post created successfully.",
                Data = Map(post)
            };
        }

        public async Task<BaseResponse<PostDto>> UpdatePostAsync(string userId, EditPostDto dto)
        {
            _logger.LogInformation("UserId={UserId} editing post {PostId}", userId, dto.Id);
            var post = await _repo.GetByIdAsync(dto.Id);
            if (post == null || post.AuthorId != userId)
            {
                _logger.LogWarning("Edit denied for PostId={PostId}, UserId={UserId}", dto.Id, userId);
                return new BaseResponse<PostDto> { Success = false, Message = "Edit failed." };
            }

            post.Title = dto.Title;
            post.Content = dto.Content;
            await _repo.UpdateAsync(post);

            try
            {
                await _repo.SaveChangesAsync();
                _logger.LogInformation("Updated post {PostId}", dto.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating post {PostId}", dto.Id);
                return new BaseResponse<PostDto> { Success = false, Message = $"Error updating post: {ex.Message}" };
            }

            return new BaseResponse<PostDto>
            {
                Success = true,
                Message = "Post updated successfully.",
                Data = Map(post)
            };
        }

        public async Task<BaseResponse<object>> DeletePostAsync(string userId, int postId, bool isAdmin)
        {
            _logger.LogInformation("UserId={UserId} deleting post {PostId} (isAdmin={IsAdmin})",
                userId, postId, isAdmin);

            var post = await _repo.GetByIdAsync(postId);
            if (post == null || (!isAdmin && post.AuthorId != userId))
            {
                _logger.LogWarning("Delete denied for PostId={PostId}, UserId={UserId}", postId, userId);
                return new BaseResponse<object> { Success = false, Message = "Post not found or access denied." };
            }

            await _repo.DeleteAsync(post);
            try
            {
                await _repo.SaveChangesAsync();
                _logger.LogInformation("Deleted post {PostId}", postId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting post {PostId}", postId);
                return new BaseResponse<object> { Success = false, Message = $"Error deleting post: {ex.Message}" };
            }

            return new BaseResponse<object> { Success = true, Message = "Post deleted successfully." };
        }

        public async Task<BaseResponse<object>> UpvoteAsync(string userId, int postId)
        {
            _logger.LogInformation("UserId={UserId} upvoting PostId={PostId}", userId, postId);
            var post = await _repo.GetByIdAsync(postId);
            if (post == null)
            {
                _logger.LogWarning("Upvote failed: PostId={PostId} not found", postId);
                return new BaseResponse<object> { Success = false, Message = "Post not found." };
            }

            var existing = post.Votes.FirstOrDefault(v => v.UserId == userId);
            if (existing != null) existing.IsUpvote = true;
            else post.Votes.Add(new Vote { PostId = postId, UserId = userId, IsUpvote = true });

            try
            {
                await _repo.SaveChangesAsync();
                _logger.LogInformation("Upvoted post {PostId}", postId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error upvoting post {PostId}", postId);
                return new BaseResponse<object> { Success = false, Message = $"Error upvoting post: {ex.Message}" };
            }

            return new BaseResponse<object> { Success = true, Message = "Post upvoted successfully." };
        }

        public async Task<BaseResponse<object>> DownvoteAsync(string userId, int postId)
        {
            _logger.LogInformation("UserId={UserId} downvoting PostId={PostId}", userId, postId);
            var post = await _repo.GetByIdAsync(postId);
            if (post == null)
            {
                _logger.LogWarning("Downvote failed: PostId={PostId} not found", postId);
                return new BaseResponse<object> { Success = false, Message = "Post not found." };
            }

            var existing = post.Votes.FirstOrDefault(v => v.UserId == userId);
            if (existing != null) existing.IsUpvote = false;
            else post.Votes.Add(new Vote { PostId = postId, UserId = userId, IsUpvote = false });

            try
            {
                await _repo.SaveChangesAsync();
                _logger.LogInformation("Downvoted post {PostId}", postId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error downvoting post {PostId}", postId);
                return new BaseResponse<object> { Success = false, Message = $"Error downvoting post: {ex.Message}" };
            }

            return new BaseResponse<object> { Success = true, Message = "Post downvoted successfully." };
        }

        public async Task<BaseResponse<object>> CommentAsync(string userId, CommentDto dto)
        {
            _logger.LogInformation("UserId={UserId} commenting on PostId={PostId}", userId, dto.PostId);
            var post = await _repo.GetByIdAsync(dto.PostId);
            if (post == null)
            {
                _logger.LogWarning("Comment failed: PostId={PostId} not found", dto.PostId);
                return new BaseResponse<object> { Success = false, Message = "Post not found." };
            }

            post.Comments.Add(new Comment { PostId = dto.PostId, UserId = userId, Text = dto.Text });

            try
            {
                await _repo.SaveChangesAsync();
                _logger.LogInformation("Comment added to PostId={PostId}", dto.PostId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error commenting on PostId={PostId}", dto.PostId);
                return new BaseResponse<object> { Success = false, Message = $"Error adding comment: {ex.Message}" };
            }

            return new BaseResponse<object> { Success = true, Message = "Comment added successfully." };
        }

        private static PostDto Map(Post p) => new PostDto
        {
            Id = p.Id,
            Title = p.Title,
            Content = p.Content,
            AuthorId = p.AuthorId,
            CreatedAt = p.CreatedAt,
            VoteCount = p.Votes.Count(v => v.IsUpvote) - p.Votes.Count(v => !v.IsUpvote),
            CommentCount = p.Comments.Count,
            Comments = p.Comments.Select(c => new CommentDetailDto
            {
                Id = c.Id,
                UserId = c.UserId,
                Text = c.Text,
                CreatedAt = c.CreatedAt
            }).ToList()
        };
    }
}
