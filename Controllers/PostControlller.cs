using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Talorants.Entities;
using Talorants.Models;
using Talorants.Services;

namespace Talorants.Controllers;

[Authorize]
public class PostController : Controller
{
    private readonly ILogger<PostController> _logger;
    private readonly IPostService _services;
    private readonly UserManager<User> _userManager;

    public PostController(
        ILogger<PostController> logger,
        IPostService service,
        UserManager<User> userManager
    )
    {
        _logger = logger;
        _services = service;
        _userManager = userManager;
    }

    public IActionResult CreatePost() => View();

    [HttpPost]
    public async Task<IActionResult> CreatePost(CreateOrUpdatePostViewModel model)
    {
        if(!ModelState.IsValid) return View();
        var user = await _userManager.FindByNameAsync(User.Identity.Name);
        var post = await _services.CreatePostAsync(model, user.Id);
        return LocalRedirect($"/post/article/{post.Id}");
    }

    public async Task<IActionResult> Article([FromRoute]long id)
    {
        var post = await _services.GetPostAsync(id);
        return View(post);
    }
}