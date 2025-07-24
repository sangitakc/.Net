using BlogManagementSystem.Api.DTOs;

namespace BlogManagementSystem.Api.Services
{
    public interface IUserService
    {
        Task<BaseResponse<AuthorDto>> GetOwnUserAsync(string userId);
        Task<BaseResponse<AuthorDto>> UpdateOwnUserAsync(string userId, EditUserDto dto);
        
    }
}
