using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using BlogManagementSystem.Api.Controllers;
using BlogManagementSystem.Api.Services;
using BlogManagementSystem.Api.DTOs;

namespace BlogManagementSystem.Tests.Controllers
{
    public class PostsControllerTests
    {
        private readonly IPostService _service = A.Fake<IPostService>();
        private readonly PostsController _controller;

        public PostsControllerTests()
        {
            _controller = new PostsController(_service);
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "user1")
            }, "mock"));
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [Fact]
        public async Task GetAll_ShouldReturnOkWithPosts()
        {
            var dtos = new List<PostDto> { new PostDto { Id=1, Title="T" } };
            A.CallTo(() => _service.GetAllPostsAsync()).Returns(Task.FromResult<IEnumerable<PostDto>>(dtos));

            var result = await _controller.GetAll();
            var ok = result.Result as OkObjectResult;
            ok.Should().NotBeNull();
            var response = ok.Value as BaseResponse<object>;
            response.Success.Should().BeTrue();
        }
    }
}
