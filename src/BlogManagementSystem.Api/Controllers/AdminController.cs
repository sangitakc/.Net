using BlogManagementSystem.Api.DTOs;
using BlogManagementSystem.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlogManagementSystem.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _admin;

        public AdminController(IAdminService admin)
        {
            _admin = admin;
        }


        [HttpDelete("delete_any_post/{id}")]
        public async Task<IActionResult> DeleteAnyPost(int id)
        {
            var res = await _admin.DeleteAnyPostAsync(id);
            return res.Success ? Ok(res) : NotFound(res);
        }



        [HttpDelete("delete_author/{id}")]
        public async Task<IActionResult> DeleteAuthor(string id)
        {
            var res = await _admin.DeleteAuthorAsync(id);
            return res.Success ? Ok(res) : NotFound(res);
        }



        [HttpGet("view_author_information/{id}")]
        public async Task<IActionResult> ViewAuthorInformation(string id)
        {
            var res = await _admin.ViewAuthorInformationAsync(id);
            return res.Success ? Ok(res) : NotFound(res);
        }



        [HttpGet("view_all_authors")]
        public async Task<ActionResult<BaseResponse<IEnumerable<AuthorDto>>>> ViewAllAuthors()
        {
            var res = await _admin.ViewAllAuthorsAsync();
            return Ok(res);
        }
    }
}
