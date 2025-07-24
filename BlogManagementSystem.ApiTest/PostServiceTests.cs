using BlogManagementSystem.Api.DTOs;
using BlogManagementSystem.Api.Models;
using BlogManagementSystem.Api.Repositories;
using BlogManagementSystem.Api.Services;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogManagementSystem.ApiTest
{
    public class PostServiceTests
    {
        private readonly IPostRepository _repo;
        private readonly PostService _service;

        public PostServiceTests()
        {
            _repo = A.Fake<IPostRepository>();
            var log = A.Fake<ILogger<PostService>>();
            _service = new PostService(_repo, log);
        }

        [Fact]
        public async Task GetAllPostsAsync_Succeeds()
        {
            var posts = new List<Post> {
                new Post {
                    Id = 1,
                    Title = "T",
                    Content = "C",
                    AuthorId = "U",
                    Votes = new List<Vote> { new Vote { IsUpvote = true, UserId = "u" } },
                    Comments = new List<Comment> { new Comment() }
                }
            };
            A.CallTo(() => _repo.GetAllAsync()).Returns(posts);

            var result = await _service.GetAllPostsAsync();

            Assert.True(result.Success);
            Assert.Single(result.Data);
            Assert.Equal(1, result.Data.First().VoteCount);
            Assert.Equal(1, result.Data.First().CommentCount);
        }

        [Fact]
        public async Task CreatePostAsync_Succeeds()
        {
            var post = new Post { Id = 5, Title = "Hello", Content = "World", AuthorId = "U" };
            A.CallTo(() => _repo.AddAsync(A<Post>.Ignored))
             .Invokes((Post p) => { p.Id = post.Id; p.CreatedAt = p.CreatedAt; });
            A.CallTo(() => _repo.SaveChangesAsync()).Returns(Task.CompletedTask);

            var dto = new CreatePostDto { Title = "Hello", Content = "World" };
            var result = await _service.CreatePostAsync("U", dto);

            Assert.True(result.Success);
            Assert.Equal(5, result.Data.Id);
            Assert.Equal("Hello", result.Data.Title);
        }

        [Fact]
        public async Task UpdatePostAsync_Succeeds()
        {
            var existing = new Post { Id = 7, Title = "Old", Content = "X", AuthorId = "U" };
            A.CallTo(() => _repo.GetByIdAsync(7)).Returns(existing);
            A.CallTo(() => _repo.SaveChangesAsync()).Returns(Task.CompletedTask);

            var dto = new EditPostDto { Id = 7, Title = "New", Content = "Y" };
            var result = await _service.UpdatePostAsync("U", dto);

            Assert.True(result.Success);
            Assert.Equal("New", result.Data.Title);
            Assert.Equal("Y", result.Data.Content);
        }

        [Fact]
        public async Task DeletePostAsync_AsOwner_Succeeds()
        {
            var p = new Post { Id = 9, AuthorId = "U" };
            A.CallTo(() => _repo.GetByIdAsync(9)).Returns(p);
            A.CallTo(() => _repo.SaveChangesAsync()).Returns(Task.CompletedTask);

            var result = await _service.DeletePostAsync("U", 9, isAdmin: false);
            Assert.True(result.Success);
        }

        [Fact]
        public async Task UpvoteAsync_Succeeds()
        {
            var p = new Post { Id = 3, Votes = new List<Vote>() };
            A.CallTo(() => _repo.GetByIdAsync(3)).Returns(p);
            A.CallTo(() => _repo.SaveChangesAsync()).Returns(Task.CompletedTask);

            var result = await _service.UpvoteAsync("U", 3);
            Assert.True(result.Success);
            Assert.Single(p.Votes);
        }

        [Fact]
        public async Task DownvoteAsync_Succeeds()
        {
            var p = new Post { Id = 4, Votes = new List<Vote>() };
            A.CallTo(() => _repo.GetByIdAsync(4)).Returns(p);
            A.CallTo(() => _repo.SaveChangesAsync()).Returns(Task.CompletedTask);

            var result = await _service.DownvoteAsync("U", 4);
            Assert.True(result.Success);
            Assert.Single(p.Votes);
            Assert.False(p.Votes.First().IsUpvote);
        }

        [Fact]
        public async Task CommentAsync_Succeeds()
        {
            var p = new Post { Id = 10, Comments = new List<Comment>() };
            A.CallTo(() => _repo.GetByIdAsync(10)).Returns(p);
            A.CallTo(() => _repo.SaveChangesAsync()).Returns(Task.CompletedTask);

            var dto = new CommentDto { PostId = 10, Text = "Nice" };
            var result = await _service.CommentAsync("U", dto);

            Assert.True(result.Success);
            Assert.Single(p.Comments);
            Assert.Equal("Nice", p.Comments.First().Text);
        }
    }
}