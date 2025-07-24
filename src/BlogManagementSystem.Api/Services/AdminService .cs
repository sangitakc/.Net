using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogManagementSystem.Api.DTOs;
using BlogManagementSystem.Api.Models;
using BlogManagementSystem.Api.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace BlogManagementSystem.Api.Services
{
    public class AdminService : IAdminService
    {
        private readonly IPostRepository _postRepo;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<AdminService> _logger;

        public AdminService(
            IPostRepository postRepo,
            UserManager<ApplicationUser> userManager,
            ILogger<AdminService> logger)
        {
            _postRepo = postRepo;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<BaseResponse<object>> DeleteAnyPostAsync(int postId)
        {
            _logger.LogInformation("Admin deleting post {PostId}", postId);
            var post = await _postRepo.GetByIdAsync(postId);
            if (post == null)
            {
                _logger.LogWarning("DeleteAnyPost failed: post {PostId} not found", postId);
                return new BaseResponse<object> { Success = false, Message = "Post not found." };
            }

            await _postRepo.DeleteAsync(post);
            try
            {
                await _postRepo.SaveChangesAsync();
                _logger.LogInformation("Post {PostId} deleted by admin", postId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting post {PostId}", postId);
                return new BaseResponse<object> { Success = false, Message = $"Error deleting post: {ex.Message}" };
            }

            return new BaseResponse<object> { Success = true, Message = "Post deleted successfully." };
        }

        public async Task<BaseResponse<object>> DeleteAuthorAsync(string authorId)
        {
            _logger.LogInformation("Admin deleting author {AuthorId}", authorId);
            var user = await _userManager.FindByIdAsync(authorId);
            if (user == null)
            {
                _logger.LogWarning("DeleteAuthor failed: author {AuthorId} not found", authorId);
                return new BaseResponse<object> { Success = false, Message = "Author not found." };
            }

            var allPosts = await _postRepo.GetAllAsync();
            foreach (var p in allPosts.Where(p => p.AuthorId == authorId))
                await _postRepo.DeleteAsync(p);

            try
            {
                await _postRepo.SaveChangesAsync();
                _logger.LogInformation("Deleted posts of author {AuthorId}", authorId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting posts of author {AuthorId}", authorId);
                return new BaseResponse<object> { Success = false, Message = $"Error deleting author's posts: {ex.Message}" };
            }

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                var errs = string.Join("; ", result.Errors.Select(e => e.Description));
                _logger.LogError("Error deleting author {AuthorId}: {Errors}", authorId, errs);
                return new BaseResponse<object> { Success = false, Message = errs };
            }

            _logger.LogInformation("Author {AuthorId} deleted successfully", authorId);
            return new BaseResponse<object> { Success = true, Message = "Author deleted successfully." };
        }

        public async Task<BaseResponse<AuthorDto>> ViewAuthorInformationAsync(string authorId)
        {
            _logger.LogInformation("Admin viewing info for author {AuthorId}", authorId);
            var u = await _userManager.FindByIdAsync(authorId);
            if (u == null)
            {
                _logger.LogWarning("ViewAuthorInformation failed: author {AuthorId} not found", authorId);
                return new BaseResponse<AuthorDto> { Success = false, Message = "Author not found." };
            }

            return new BaseResponse<AuthorDto>
            {
                Success = true,
                Message = "Author info retrieved successfully.",
                Data = new AuthorDto
                {
                    Id = u.Id,
                    UserName = u.UserName!,
                    Email = u.Email!,
                    FirstName = u.FirstName,
                    LastName = u.LastName
                }
            };
        }

        public async Task<BaseResponse<IEnumerable<AuthorDto>>> ViewAllAuthorsAsync()
        {
            _logger.LogInformation("Admin retrieving all authors");
            var authors = await _userManager.GetUsersInRoleAsync("Author");
            var dtos = authors.Select(u => new AuthorDto
            {
                Id = u.Id,
                UserName = u.UserName!,
                Email = u.Email!,
                FirstName = u.FirstName,
                LastName = u.LastName
            });

            return new BaseResponse<IEnumerable<AuthorDto>>
            {
                Success = true,
                Message = "Authors retrieved successfully.",
                Data = dtos
            };
        }
    }
}
