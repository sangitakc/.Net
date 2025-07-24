using System.Collections.Generic;
using System.Threading.Tasks;
using BlogManagementSystem.Api.Models;
using BlogManagementSystem.Api.Repositories;
using BlogManagementSystem.Api.Services;
using FakeItEasy;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Xunit;

namespace BlogManagementSystem.ApiTest
{
    public class AdminServiceTests
    {
        private readonly IPostRepository _repo;
        private readonly UserManager<ApplicationUser> _users;
        private readonly AdminService _svc;

        public AdminServiceTests()
        {
          
            _repo = A.Fake<IPostRepository>();

           
            var fakeStore = A.Fake<IUserStore<ApplicationUser>>();
            _users = A.Fake<UserManager<ApplicationUser>>(opts => opts
                .WithArgumentsForConstructor(new object?[]
                {
                    fakeStore,
                    null, null, null, null, null, null, null, null
                }));

            _svc = new AdminService(_repo, _users, A.Fake<ILogger<AdminService>>());
        }

        [Fact]
        public async Task DeleteAnyPostAsync_Succeeds()
        {
            
            var p = new Post { Id = 2 };
            A.CallTo(() => _repo.GetByIdAsync(2)).Returns(p);
            A.CallTo(() => _repo.SaveChangesAsync()).Returns(Task.CompletedTask);

        
            var res = await _svc.DeleteAnyPostAsync(2);

           
            Assert.True(res.Success);
        }

        [Fact]
        public async Task DeleteAuthorAsync_Succeeds()
        {
          
            var user = new ApplicationUser { Id = "a" };
            A.CallTo(() => _users.FindByIdAsync("a")).Returns(Task.FromResult(user));
            A.CallTo(() => _repo.GetAllAsync())
             .Returns(new List<Post> { new Post { AuthorId = "a" } });
            A.CallTo(() => _repo.SaveChangesAsync()).Returns(Task.CompletedTask);
            A.CallTo(() => _users.DeleteAsync(user))
             .Returns(Task.FromResult(IdentityResult.Success));

        
            var res = await _svc.DeleteAuthorAsync("a");

   
            Assert.True(res.Success);
        }

        [Fact]
        public async Task ViewAuthorInformationAsync_Succeeds()
        {
          
            var user = new ApplicationUser
            {
                Id = "x",
                UserName = "u",
                Email = "e",
                FirstName = "A",
                LastName = "B"
            };
            A.CallTo(() => _users.FindByIdAsync("x")).Returns(Task.FromResult(user));

   
            var res = await _svc.ViewAuthorInformationAsync("x");

            Assert.True(res.Success);
            Assert.Equal("u", res.Data.UserName);
        }

        [Fact]
        public async Task ViewAllAuthorsAsync_Succeeds()
        {

            var list = new List<ApplicationUser>
            {
                new ApplicationUser { Id = "1", UserName = "u1", Email = "e1", FirstName = "F", LastName = "L" }
            };
            A.CallTo(() => _users.GetUsersInRoleAsync("Author"))
             .Returns(Task.FromResult((IList<ApplicationUser>)list));

            var res = await _svc.ViewAllAuthorsAsync();

  
            Assert.True(res.Success);
            Assert.Single(res.Data);
        }
    }
}
