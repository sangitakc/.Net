using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using Xunit;
using BlogManagementSystem.Api.Repositories;
using BlogManagementSystem.Api.Services;
using BlogManagementSystem.Api.DTOs;
using BlogManagementSystem.Api.Models;
using System.Collections.Generic;

namespace BlogManagementSystem.Tests.Services
{
    public class PostServiceTests
    {
        private readonly IPostRepository _repo = A.Fake<IPostRepository>();
        private readonly PostService _service;

        public PostServiceTests()
        {
            _service = new PostService(_repo);
        }

        [Fact]
        public async Task CreatePostAsync_ShouldAddAndReturnPostDto()
        {
            var dto = new CreatePostDto { Title = "Test", Content = "Content" };
            A.CallTo(() => _repo.AddAsync(A<Post>._)).Returns(Task.CompletedTask);
            A.CallTo(() => _repo.SaveChangesAsync()).Returns(Task.CompletedTask);

            var result = await _service.CreatePostAsync("user1", dto);

            result.Should().NotBeNull();
            result.Title.Should().Be(dto.Title);
            A.CallTo(() => _repo.AddAsync(A<Post>.That.Matches(p => p.Title == dto.Title && p.Content == dto.Content && p.AuthorId == "user1"))).MustHaveHappened();
        }

        [Fact]
        public async Task GetAllPostsAsync_ShouldReturnMappedDtos()
        {
            var posts = new List<Post>
            {
                new Post { Id = 1, Title="T1", Content="C1", AuthorId="u1", Votes=new List<Vote>{ new Vote{ IsUpvote=true, UserId="u1"} }, Comments=new List<Comment>{ new Comment() }},
                new Post { Id = 2, Title="T2", Content="C2", AuthorId="u2" }
            };
            A.CallTo(() => _repo.GetAllAsync()).Returns(Task.FromResult<IEnumerable<Post>>(posts));

            var result = await _service.GetAllPostsAsync();

            result.Should().HaveCount(2);
            result.Should().Contain(r => r.Id == 1 && r.VoteCount == 1 && r.CommentCount == 1);
        }
    }
}
