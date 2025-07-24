// IPostService.cs
using BlogManagementSystem.Api.DTOs;
using BlogManagementSystem.Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlogManagementSystem.Api.Services
{
    public interface IPostService
    {
        Task<BaseResponse<IEnumerable<PostDto>>> GetAllPostsAsync();
        Task<BaseResponse<PostDto>> GetPostByIdAsync(int postId);
        Task<BaseResponse<PostDto>> CreatePostAsync(string userId, CreatePostDto dto);
        Task<BaseResponse<PostDto>> UpdatePostAsync(string userId, EditPostDto dto);
        Task<BaseResponse<object>> DeletePostAsync(string userId, int postId, bool isAdmin);
        Task<BaseResponse<object>> UpvoteAsync(string userId, int postId);
        Task<BaseResponse<object>> DownvoteAsync(string userId, int postId);
        Task<BaseResponse<object>> CommentAsync(string userId, CommentDto dto);
        Task<BaseResponse<IEnumerable<PostDto>>> GetPostsByAuthorAsync(string authorId);

    }
}
