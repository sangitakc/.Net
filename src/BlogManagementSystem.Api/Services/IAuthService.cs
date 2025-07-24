using BlogManagementSystem.Api.DTOs;

namespace BlogManagementSystem.Api.Services
{
    public interface IAuthService
    {
        Task<BaseResponse<string>> RegisterAsync(RegisterDto dto);
        Task<BaseResponse<string>> LoginAsync(LoginDto dto);
    }
}
