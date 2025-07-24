// src/BlogManagementSystem.Api/Services/IAdminService.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using BlogManagementSystem.Api.DTOs;

namespace BlogManagementSystem.Api.Services
{
    public interface IAdminService
    {
        Task<BaseResponse<object>> DeleteAnyPostAsync(int postId);
        Task<BaseResponse<object>> DeleteAuthorAsync(string authorId);
        Task<BaseResponse<AuthorDto>> ViewAuthorInformationAsync(string authorId);
        Task<BaseResponse<IEnumerable<AuthorDto>>> ViewAllAuthorsAsync();
    }
}
