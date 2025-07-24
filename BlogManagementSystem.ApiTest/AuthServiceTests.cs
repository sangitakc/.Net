using System.Collections.Generic;
using System.Threading.Tasks;
using BlogManagementSystem.Api.DTOs;
using BlogManagementSystem.Api.Models;
using BlogManagementSystem.Api.Services;
using FakeItEasy;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Xunit;

namespace BlogManagementSystem.ApiTest
{
    public class AuthServiceTests
    {
        private readonly UserManager<ApplicationUser> _users;
        private readonly RoleManager<IdentityRole> _roles;
        private readonly IConfiguration _config;
        private readonly AuthService _svc;

        public AuthServiceTests()
        {
            
            var fakeUserStore = A.Fake<IUserStore<ApplicationUser>>();
            _users = A.Fake<UserManager<ApplicationUser>>(opts => opts
                .WithArgumentsForConstructor(new object?[] { fakeUserStore, null, null, null, null, null, null, null, null }));
            var fakeRoleStore = A.Fake<IRoleStore<IdentityRole>>();
            _roles = A.Fake<RoleManager<IdentityRole>>(opts => opts
                .WithArgumentsForConstructor(new object?[] { fakeRoleStore, null, null, null, null })); 


            var fakeJwtSection = A.Fake<IConfigurationSection>();

            _config = A.Fake<IConfiguration>();
            A.CallTo(() => _config.GetSection("Jwt"))
             .Returns(fakeJwtSection);

            
            A.CallTo(() => fakeJwtSection["Key"]).Returns("01234567890123456789012345678901");
            A.CallTo(() => fakeJwtSection["Issuer"]).Returns("I");
            A.CallTo(() => fakeJwtSection["Audience"]).Returns("A");
            A.CallTo(() => fakeJwtSection["ExpiresInMinutes"]).Returns("60");

           
            _svc = new AuthService(_users, _roles, _config);
        }



        [Fact]
        public async Task RegisterAsync_NewUser_Succeeds()
        {
           
            A.CallTo(() => _users.FindByNameAsync("u")).Returns(Task.FromResult<ApplicationUser>(null));
            A.CallTo(() => _users.FindByEmailAsync("e")).Returns(Task.FromResult<ApplicationUser>(null));
            A.CallTo(() => _users.CreateAsync(A<ApplicationUser>.Ignored, "P1!"))
             .Returns(Task.FromResult(IdentityResult.Success));
            A.CallTo(() => _roles.RoleExistsAsync("Author")).Returns(Task.FromResult(true));
            A.CallTo(() => _users.AddToRoleAsync(A<ApplicationUser>.Ignored, "Author"))
             .Returns(Task.FromResult(IdentityResult.Success));

            
            var res = await _svc.RegisterAsync(new RegisterDto { Username = "u", Email = "e", Password = "P1!" });

           
            Assert.True(res.Success);
        }

        [Fact]
        public async Task LoginAsync_ValidCredentials_Succeeds()
        {
           
            var user = new ApplicationUser { Id = "1", FirstName = "A", LastName = "B", Email = "e" };
            A.CallTo(() => _users.FindByNameAsync("u")).Returns(Task.FromResult(user));
            A.CallTo(() => _users.CheckPasswordAsync(user, "P1!")).Returns(Task.FromResult(true));
            A.CallTo(() => _users.GetRolesAsync(user))
             .Returns(Task.FromResult((IList<string>)new List<string> { "Author" }));

         
            var res = await _svc.LoginAsync(new LoginDto { Username = "u", Password = "P1!" });

            Assert.True(res.Success);
            Assert.False(string.IsNullOrEmpty(res.Data));
        }
    }
}
