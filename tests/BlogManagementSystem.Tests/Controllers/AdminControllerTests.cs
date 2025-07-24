using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using BlogManagementSystem.Api.Controllers;
using BlogManagementSystem.Api.Services;
using BlogManagementSystem.Api.DTOs;

namespace BlogManagementSystem.Tests.Controllers
{
    public class AdminControllerTests
    {
        private readonly IPostService _service = A.Fake<IPostService>();
        private readonly UserManager<IdentityUser> _userManager;
        private readonly AdminController _controller;

        public AdminControllerTests()
        {
            var store = A.Fake<IUserStore<IdentityUser>>();
            _userManager = new UserManager<IdentityUser>(store, null, null, null, null, null, null, null, null);
            _controller = new AdminController(_service, _userManager);
        }

        [Fact]
        public async Task DeleteAny_ShouldReturnNotFound_WhenPostMissing()
        {
            A.CallTo(() => _service.DeleteAnyPostAsync(1)).Returns(false);

            var result = await _controller.DeleteAny(1);
            var notFound = result.Result as NotFoundObjectResult;
            notFound.Should().NotBeNull();
        }
    }
}
