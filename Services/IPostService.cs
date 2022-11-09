using Talorants.Models;

namespace Talorants.Services;

public interface IPostService
{
    Task<PostViewModel> CreatePostAsync(CreateOrUpdatePostViewModel model, string userId);
    Task<PostViewModel> UpdatePostAsync(long id, CreateOrUpdatePostViewModel model);
    Task<PostViewModel> GetPostAsync(long id);
    Task<IEnumerable<PostViewModel>> GetAllPostsAsync();
    Task DeletePostAsync(long id);
}