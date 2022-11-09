using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Talorants.ApiRequest;
using Talorants.Blog.Models;
using Talorants.Entities;
using Talorants.Extensions;
using Talorants.Models;
using Talorants.Repositories;
using System;
using System.Net.Mail;
using System.IO;
using System.Configuration;
using System.Web;
using Talorants.Services;

namespace Talorants.Controllers;

public class AccountController : Controller
{
    private readonly ILogger<AccountController> _logger;
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly ISendEmailService _sendEmailService;

    public AccountController(
        ILogger<AccountController> logger,
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        ISendEmailService sendEmailService)
    {
        _logger = logger;
        _userManager = userManager;
        _signInManager = signInManager;
        _sendEmailService = sendEmailService;
    }

    public IActionResult Register(string returnUrl) => View(new SignUpViewModel() { ReturnUrl = returnUrl });

    public IActionResult LogIn(string returnUrl) => View(new SignInViewModel() { ReturnUrl = returnUrl });

    [HttpPost]
    public async Task<IActionResult> Register(SignUpViewModel model)
    {
        if(!ModelState.IsValid) return View(model);

        var user = model.ToEntity();

        var createdUser = await _userManager.CreateAsync(user, model.Password);

        if(!createdUser.Succeeded) return View(model);

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

        var callbackUrl = Url.Action("EmailConfirm", "Account",
            new { userId = user.Id, token }, Request.Scheme);

        _logger.Log(LogLevel.Warning, callbackUrl);

        _sendEmailService.EmailConfirm(user, callbackUrl);

        return LocalRedirect($"/account/login");
    }

    [HttpPost]
    public async Task<IActionResult> LogIn(SignInViewModel model)
    {
        if(!ModelState.IsValid) return View(model);

        var user = await _userManager.FindByNameAsync(model.UserName);
        if(user is null) return View(model);

        var findUser = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);
        if(!findUser.Succeeded) return View(model);

        return LocalRedirect($"{model.ReturnUrl ?? "/"}");
    }

    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction(nameof(LogIn));
    }

    public IActionResult ForgotPassword() => View();

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> ForgotPassword(ForgetpasswordViewModel model)
    {
        if(!ModelState.IsValid) return View(model);
        
        var user = await _userManager.FindByEmailAsync(model.Email);
        if(user is null) return View(model);

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        _logger.LogInformation($"{token}");

        string passwordResetLink = Url.Action("ResetPassword","Account",
            new { email = model.Email, token }, Request.Scheme);

        // start IsendEmailService
        _sendEmailService.ResetPassword(user, passwordResetLink);

        return View("ForgotPasswordConfirmation");
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult ResetPassword(string token, string email)
    {
        if(token == null || email == null)
        {
            ModelState.AddModelError("", "Xato parol tokeni=====");
        }
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
    {
        if(!ModelState.IsValid) return View(model);
        
        var user = await _userManager.FindByEmailAsync(model.Email);
        if(user is null) return View("Register");

            
        var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
        if(result.Succeeded)
        {
            return View("ResetPasswordConfirmation");
        }
        foreach(var error in result.Errors)
        {
            ModelState.AddModelError("", error.Description);
        }
        return View(model);
    }


    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> EmailConfirmAsync(string userId, string token)
    {
        if(userId is null || token is null) return RedirectToAction("index", "home");

        var user = await _userManager.FindByIdAsync(userId);

        if(user is null)
        {
            ViewBag.ErrorMessage = $"The User ID {userId} is invalid";
            return View("NotFound");
        }

        var result = await _userManager.ConfirmEmailAsync(user, token);

        if(result.Succeeded) return View();

        ViewBag.ErrorTitle = "Email cannot be confirmed";
        return View("Error");
    } 
}