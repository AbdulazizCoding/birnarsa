using System.Security.Cryptography;
using Talorants.Entities;
using Talorants.Models;

namespace Talorants.Extensions;

public static class Mappings
{
    public static User ToEntity(this SignUpViewModel model)
        => new User()
        {
            UserName = model.UserName,
            Email = model.Email,
            Avatar = SaveImage(model.Avatar)
        };

    public static Post ToEntity(this CreateOrUpdatePostViewModel model)
        => new()
        {
            Title = model.Title,
            Content = model.Content,
            Image = SaveImage(model.Image)
        };

    public static PostViewModel ToModel(this Post entity)
        => new()
        {
            Id = entity.Id,
            Title = entity.Title,
            Content = entity.Content,
            Image = entity.Image,
            AppUserId = entity.UserId,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            IsEdited = entity.IsEdited
        };

    // public static string ToBase64String(IFormFile image)
    // {
    //     var memoryStream = new MemoryStream();
    //     image.CopyToAsync(memoryStream);
    //     var result = memoryStream.ToArray();
    //     while(result.Count() == 0) result = memoryStream.ToArray();
    //     var str = Convert.ToBase64String(result);
    //     return "data:image/jpeg;base64,"+str;
    // }

    public static string SaveImage(IFormFile image)
    {
        if(image is null) return "avatar.jpg";

        var imgPath = Guid.NewGuid().ToString("N") + Path.GetExtension(image.FileName);
        
        var memoryStream = new MemoryStream();
        image.CopyTo(memoryStream);
            

        return imgPath;
    }
}