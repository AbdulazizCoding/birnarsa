using System.ComponentModel.DataAnnotations.Schema;

namespace Talorants.Entities;

public class Category
{
    public string Id { get; set; }
    public string Name { get; set; }
    [ForeignKey(nameof(Post))]
    public List<Post> Posts { get; set; }
}