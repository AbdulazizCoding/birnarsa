using Microsoft.AspNetCore.Identity;
using Talorants.Entities;
using Talorants.Extensions;
using Talorants.Models;
using Talorants.Repositories;

namespace Talorants.Services;

public class PostService : IPostService
{
    private readonly UserManager<User> _userManager;
    private readonly ILogger<PostService> _logger;
    private readonly IPostRepository _repository;

    public PostService(
        ILogger<PostService> logger,
        IPostRepository repository,
        UserManager<User> userManager)
    {
        _logger = logger;
        _repository = repository;
        _userManager = userManager;
    }

    public async Task<PostViewModel> CreatePostAsync(CreateOrUpdatePostViewModel model, string userId)
    {
        var entity = await _repository.CreateAsync(model.ToEntity(), userId);
        return entity.ToModel();
    }

    public async Task DeletePostAsync(long id)
    {
        await _repository.DaleteAsync(id);
    }

    public async Task<IEnumerable<PostViewModel>> GetAllPostsAsync()
    {
        var posts = await _repository.GetAllPostsAsync(p => true);
        return posts.Select(p => p.ToModel());
    }

    public async Task<PostViewModel> GetPostAsync(long id)
    {
        var post = await _repository.GetPostAsync(id);
        return post.ToModel();
    }

    public async Task<PostViewModel> UpdatePostAsync(long id, CreateOrUpdatePostViewModel model)
    {
        var entity = await _repository.GetPostAsync(id);
        entity.Title = model.Title;
        entity.Content = model.Content;
        entity.Image = Mappings.SaveImage(model.Image);
        entity.UpdatedAt = DateTime.Now;
        entity.IsEdited = true;
        return entity.ToModel();
    }
}