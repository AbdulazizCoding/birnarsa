using Talorants.Entities;
using Talorants.Models;

namespace Talorants.Services;

public interface ISendEmailService
{
    void ResetPassword(User user, string passwordResetLink);
    void EmailConfirm(User user, string passwordResetLink);
}