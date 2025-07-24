using System.Threading.Tasks;
using BlogManagementSystem.Api.DTOs;
using BlogManagementSystem.Api.Models;
using BlogManagementSystem.Api.Services;
using FakeItEasy;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Xunit;

namespace BlogManagementSystem.ApiTest
{
    public class UserServiceTests
    {
        private readonly UserManager<ApplicationUser> _users;
        private readonly UserService _svc;

        public UserServiceTests()
        {
            var fakeStore = A.Fake<IUserStore<ApplicationUser>>();

         
            _users = A.Fake<UserManager<ApplicationUser>>(opts => opts
                .WithArgumentsForConstructor(new object?[]
                {
                    fakeStore,
                    null, 
                    null, 
                    null, 
                    null, 
                    null, 
                    null, 
                    null,
                    null  
                }));

         
            _svc = new UserService(_users, A.Fake<ILogger<UserService>>());
        }

        [Fact]
        public async Task GetOwnUserAsync_Found_Succeeds()
        {
            var user = new ApplicationUser
            {
                Id = "u",
                UserName = "u",
                Email = "e",
                FirstName = "A",
                LastName = "B"
            };
            A.CallTo(() => _users.FindByIdAsync("u"))
             .Returns(Task.FromResult(user));

          
            var res = await _svc.GetOwnUserAsync("u");

        
            Assert.True(res.Success);
            Assert.Equal("u", res.Data.Id);
            Assert.Equal("A", res.Data.FirstName);
        }

        [Fact]
        public async Task UpdateOwnUserAsync_Succeeds()
        {
           
            var user = new ApplicationUser
            {
                Id = "u",
                FirstName = "A",
                LastName = "B",
                Email = "e"
            };
            A.CallTo(() => _users.FindByIdAsync("u"))
             .Returns(Task.FromResult(user));
            A.CallTo(() => _users.UpdateAsync(user))
             .Returns(Task.FromResult(IdentityResult.Success));

            var dto = new EditUserDto
            {
                FirstName = "X",
                LastName = "Y",
                Email = "z@z.com"
            };

       
            var res = await _svc.UpdateOwnUserAsync("u", dto);

            
            Assert.True(res.Success);
            Assert.Equal("X", res.Data.FirstName);
            Assert.Equal("z@z.com", res.Data.Email);
        }
    }
}
