// PostsController.cs
using BlogManagementSystem.Api.DTOs;
using BlogManagementSystem.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
public class PostsController : ControllerBase
{
    private readonly IPostService _postService;
    private readonly IUserService _userService;

    public PostsController(IPostService postService, IUserService userService)
    {
        _postService = postService;
        _userService = userService;
    }

    [HttpGet("view_all_posts")]
    public async Task<ActionResult<BaseResponse<IEnumerable<PostDto>>>> ViewAllPosts()
    {
        var res = await _postService.GetAllPostsAsync();
        return res.Success ? Ok(res) : BadRequest(res);
    }


    [HttpGet("view_post/{id}"), Authorize(Roles = "Author,Admin")]
    public async Task<ActionResult<BaseResponse<PostDto>>> ViewPost(int id)
    {
        var res = await _postService.GetPostByIdAsync(id);
        return res.Success ? Ok(res) : NotFound(res);
    }

    [HttpPost("create_own_post"), Authorize(Roles = "Author")]
    public async Task<ActionResult<BaseResponse<PostDto>>> CreateOwnPost(CreatePostDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var res = await _postService.CreatePostAsync(userId, dto);
        return res.Success ? Ok(res) : BadRequest(res);
    }

    [HttpPut("edit_own_post"), Authorize(Roles = "Author")]
    public async Task<ActionResult<BaseResponse<PostDto>>> EditOwnPost(EditPostDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var res = await _postService.UpdatePostAsync(userId, dto);
        return res.Success ? Ok(res) : BadRequest(res);
    }

    [HttpDelete("delete_post/{id}"), Authorize(Roles = "Author,Admin")]
    public async Task<ActionResult<BaseResponse<object>>> DeletePost(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var isAdmin = User.IsInRole("Admin");
        var res = await _postService.DeletePostAsync(userId, id, isAdmin);
        return res.Success ? Ok(res) : NotFound(res);
    }

    [HttpPost("upvote_post/{id}"), Authorize(Roles = "Author,Admin")]
    public async Task<ActionResult<BaseResponse<object>>> UpvotePost(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var res = await _postService.UpvoteAsync(userId, id);
        return res.Success ? Ok(res) : NotFound(res);
    }

    [HttpPost("downvote_post/{id}"), Authorize(Roles = "Author,Admin")]
    public async Task<ActionResult<BaseResponse<object>>> DownvotePost(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var res = await _postService.DownvoteAsync(userId, id);
        return res.Success ? Ok(res) : NotFound(res);
    }

    [HttpPost("comment"), Authorize(Roles = "Author,Admin")]
    public async Task<ActionResult<BaseResponse<object>>> Comment(CommentDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var res = await _postService.CommentAsync(userId, dto);
        return res.Success ? Ok(res) : NotFound(res);
    }

    [HttpGet("view_own_userdetails"), Authorize(Roles = "Author")]
    public async Task<ActionResult<BaseResponse<AuthorDto>>> ViewOwnUserDetails()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var res = await _userService.GetOwnUserAsync(userId);
        return res.Success ? Ok(res) : NotFound(res);
    }

   
    [HttpPut("edit_own_userdetails"), Authorize(Roles = "Author")]
    public async Task<ActionResult<BaseResponse<AuthorDto>>> EditOwnUserDetails([FromBody] EditUserDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var response = await _userService.UpdateOwnUserAsync(userId, dto);
        return response.Success ? Ok(response) : BadRequest(response);
    }


    [HttpGet("view_my_posts"), Authorize(Roles = "Author")]
    public async Task<ActionResult<BaseResponse<IEnumerable<PostDto>>>> ViewMyPosts()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new BaseResponse<IEnumerable<PostDto>>
            {
                Success = false,
                Message = "User is not authenticated."
            });
        }

        var response = await _postService.GetPostsByAuthorAsync(userId);
        return response.Success
            ? Ok(response)
            : BadRequest(response);
    }

}
