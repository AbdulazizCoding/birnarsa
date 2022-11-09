using Microsoft.AspNetCore.Identity;

namespace Talorants.Entities;

public class User : IdentityUser
{
    public string Avatar { get; set; }
    public ICollection<Post> Posts { get; set; }
}