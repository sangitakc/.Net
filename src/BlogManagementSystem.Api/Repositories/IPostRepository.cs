using System.Collections.Generic;
using System.Threading.Tasks;
using BlogManagementSystem.Api.Models;

namespace BlogManagementSystem.Api.Repositories
{
    public interface IPostRepository
    {
        Task<IEnumerable<Post>> GetAllAsync();
        Task<Post?> GetByIdAsync(int id);
        Task AddAsync(Post post);
        Task UpdateAsync(Post post);
        Task DeleteAsync(Post post);
        Task SaveChangesAsync();

        IQueryable<Post> Query();
    }
}
