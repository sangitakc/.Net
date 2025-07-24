using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Xunit;
using BlogManagementSystem.Api.Controllers;
using BlogManagementSystem.Api.DTOs;
using System.Collections.Generic;

namespace BlogManagementSystem.Tests.Controllers
{
    public class AuthControllerTests
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _config;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            var store = A.Fake<IUserStore<IdentityUser>>();
            _userManager = new UserManager<IdentityUser>(store, null, null, null, null, null, null, null, null);
            var roleStore = A.Fake<IRoleStore<IdentityRole>>();
            _roleManager = new RoleManager<IdentityRole>(roleStore, null, null, null, null);
            var inMemorySettings = new Dictionary<string, string> {
                {"Jwt:Key","SuperSecretKey_ChangeMe123!"},
                {"Jwt:Issuer","BlogManagementSystem"},
                {"Jwt:Audience","BlogAppUsers"},
                {"Jwt:ExpiresInMinutes","60"}
            };
            _config = new ConfigurationBuilder().AddInMemoryCollection(inMemorySettings).Build();
            _controller = new AuthController(_userManager, _roleManager, _config);
        }

        [Fact]
        public async Task Register_ShouldReturnToken_OnSuccess()
        {
            var dto = new RegisterDto { Email = "test@x.com", Password = "Pass@123" };
            A.CallTo(() => _userManager.CreateAsync(A<IdentityUser>._, dto.Password))
                .Returns(Task.FromResult(IdentityResult.Success));
            A.CallTo(() => _userManager.AddToRoleAsync(A<IdentityUser>._, "Author"))
                .Returns(Task.FromResult(IdentityResult.Success));
            A.CallTo(() => _userManager.GetRolesAsync(A<IdentityUser>._))
                .Returns(Task.FromResult<IList<string>>(new List<string> { "Author" }));

            var result = await _controller.Register(dto);
            var ok = result.Result as OkObjectResult;
            ok.Should().NotBeNull();
            var response = ok.Value as BaseResponse<string>;
            response.Success.Should().BeTrue();
            response.Data.Should().NotBeNullOrEmpty();
        }
    }
}
