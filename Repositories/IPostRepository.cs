using System.Linq.Expressions;
using Talorants.Entities;

namespace Talorants.Repositories;

public interface IPostRepository
{
    Task<Post> CreateAsync(Post post, string userId);
    Task<Post> GetPostAsync(long id);
    Task<IEnumerable<Post>> GetAllPostsAsync(Expression<Func<Post, bool>> exp);
    Task<Post> UpdateAsync(Post post);
    Task DaleteAsync(long id);
}