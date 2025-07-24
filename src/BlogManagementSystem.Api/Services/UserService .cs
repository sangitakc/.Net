using System.Linq;
using System.Threading.Tasks;
using BlogManagementSystem.Api.DTOs;
using BlogManagementSystem.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace BlogManagementSystem.Api.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<UserService> _logger;

        public UserService(UserManager<ApplicationUser> userManager,
                           ILogger<UserService> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<BaseResponse<AuthorDto>> GetOwnUserAsync(string userId)
        {
            _logger.LogInformation("Retrieving profile for UserId={UserId}", userId);
            var u = await _userManager.FindByIdAsync(userId);
            if (u == null)
            {
                _logger.LogWarning("User not found: UserId={UserId}", userId);
                return new BaseResponse<AuthorDto> { Success = false, Message = "User not found." };
            }

            return new BaseResponse<AuthorDto>
            {
                Success = true,
                Message = "User retrieved successfully.",
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

        public async Task<BaseResponse<AuthorDto>> UpdateOwnUserAsync(string userId, EditUserDto dto)
        {
            _logger.LogInformation("Updating profile for UserId={UserId}", userId);
            var u = await _userManager.FindByIdAsync(userId);
            if (u == null)
            {
                _logger.LogWarning("User not found: UserId={UserId}", userId);
                return new BaseResponse<AuthorDto> { Success = false, Message = "User not found." };
            }

            u.FirstName = dto.FirstName;
            u.LastName = dto.LastName;
            u.Email = dto.Email;

            var res = await _userManager.UpdateAsync(u);
            if (!res.Succeeded)
            {
                var errs = string.Join("; ", res.Errors.Select(e => e.Description));
                _logger.LogError("Error updating user {UserId}: {Errors}", userId, errs);
                return new BaseResponse<AuthorDto> { Success = false, Message = errs };
            }

            _logger.LogInformation("Profile updated for UserId={UserId}", userId);
            return new BaseResponse<AuthorDto>
            {
                Success = true,
                Message = "User details updated.",
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

        public async Task<BaseResponse<AuthorDto>> GetUserAsync(string userId)
        {
            _logger.LogInformation("Retrieving author info for UserId={UserId}", userId);
            var u = await _userManager.FindByIdAsync(userId);
            if (u == null)
            {
                _logger.LogWarning("Author not found: UserId={UserId}", userId);
                return new BaseResponse<AuthorDto> { Success = false, Message = "User not found." };
            }

            return new BaseResponse<AuthorDto>
            {
                Success = true,
                Message = "Author retrieved successfully.",
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

        public async Task<BaseResponse<IEnumerable<AuthorDto>>> GetAllAuthorsAsync()
        {
            _logger.LogInformation("Retrieving all authors");
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
