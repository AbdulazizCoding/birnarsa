using System.Threading;
using System.Web;
using Microsoft.AspNetCore.Identity;
using Talorants.ApiRequest;
using Talorants.Entities;
using Talorants.Models;

namespace Talorants.Services;

public class SendEmailService : ISendEmailService
{
    private readonly ILogger<SendEmailService> _logger;
    private readonly UserManager<User> _userManager;

    public SendEmailService(ILogger<SendEmailService> logger, UserManager<User> userManager)
    {
        _logger = logger;
        _userManager = userManager;
    }

    public async void ResetPassword(User user, string passwordResetLink)
    {
        var fromEmail = EmailCut(user.Email);

        var subject = "Talorants.uz";

        var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "resetPassword.html");
        _logger.LogInformation($"PATH = {path}");

        var pathString = System.IO.File.ReadAllText(path);
        _logger.LogInformation($"PathString= {pathString}");

        var StringBuilder = new System.Text.StringBuilder(pathString);
        StringBuilder.Replace("{{NAME}}", user.UserName);
        StringBuilder.Replace("{{LINK}}", HttpUtility.UrlEncode(passwordResetLink));
        _logger.LogInformation(StringBuilder.ToString());


        GetRequest.client.PostAsync($"https://localhost:7166/SendEmail?subject={subject}&to={fromEmail}%40gmail.com&mailBody={StringBuilder.ToString()}", null);

        _logger.Log(LogLevel.Warning, passwordResetLink);
    }

    public async void EmailConfirm(User user, string passwordResetLink)
    {
        var fromEmail = EmailCut(user.Email);

        var subject = "Talorants.uz";

        var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "confirmEmail.html");
        _logger.LogInformation($"PATH = {path}");

        var pathString = System.IO.File.ReadAllText(path);
        _logger.LogInformation($"PathString= {pathString}");

        var StringBuilder = new System.Text.StringBuilder(pathString);
        StringBuilder.Replace("{{NAME}}", user.UserName);
        StringBuilder.Replace("{{LINK}}", HttpUtility.UrlEncode(passwordResetLink));
        _logger.LogInformation(StringBuilder.ToString());

        await GetRequest.client.PostAsync($"https://localhost:7166/SendEmail?subject={subject}&to={fromEmail}%40gmail.com&mailBody={StringBuilder.ToString()}", null);

        _logger.Log(LogLevel.Warning, passwordResetLink);
    }

    public static string EmailCut(string email)
    {
        string resultEmail = ""; 

        for (int i = 0; i < email.Length; i++)
        {
            if(email[i] == '@')
            {
                return resultEmail;
            }
            resultEmail+=email[i];
        }
        return resultEmail;
    }
}