using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BlogManagementSystem.Api.Data;
using BlogManagementSystem.Api.Models;

namespace BlogManagementSystem.Api.Repositories
{
    public class PostRepository : IPostRepository
    {
        private readonly AppDbContext _context;
        public PostRepository(AppDbContext context) => _context = context;

        public async Task<IEnumerable<Post>> GetAllAsync() =>
            await _context.Posts
        .Include(p => p.Comments)   
        .Include(p => p.Votes)       
        .ToListAsync();

        public async Task<Post?> GetByIdAsync(int id) =>
            await _context.Posts.Include(p => p.Comments).Include(p => p.Votes)
                .FirstOrDefaultAsync(p => p.Id == id);

        public async Task AddAsync(Post post) => await _context.Posts.AddAsync(post);

        public Task UpdateAsync(Post post)
        {
            _context.Posts.Update(post);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Post post)
        {
            _context.Posts.Remove(post);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
    }
}
