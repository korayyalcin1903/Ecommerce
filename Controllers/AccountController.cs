using Ecommerce.Data.Concrete;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Controllers;

public class AccountController:Controller
{
    public readonly UserManager<User> _userManager;
    public readonly SignInManager<User> _signInManager;
    public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task<IActionResult> Profile(string username)
    {
        if(username == null){
            return NotFound();
        }

        if(User.Identity.Name == username){
            var user = await _userManager.FindByNameAsync(username);
            if(user != null){
                return View(user);
            } else {
                return NotFound();
            }
        } else {
            return NotFound();
        }
    }

    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model, IFormFile formFile)
    {
        var extension = "";
        var randomFileName = "user.jpg";

        if (formFile != null){
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            extension = Path.GetExtension(formFile.FileName);

            if (!allowedExtensions.Contains(extension)){
                ModelState.AddModelError("", "Geçerli bir resim seçiniz.");
            } else {
                randomFileName = string.Format($"{Guid.NewGuid().ToString()}{extension}");
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/user", randomFileName);

                using (var stream = new FileStream(path, FileMode.Create)){
                    await formFile.CopyToAsync(stream);
                }
            }
        }

        if (ModelState.IsValid){
            var user = await _userManager.FindByEmailAsync(model.Email);
            var username = await _userManager.FindByNameAsync(model.UserName);

            if (user == null){
                if (username == null){
                    
                    user = new User{
                        Name = model.Name,
                        Surname = model.Surname,
                        UserName = model.UserName,
                        Email = model.Email,
                        Avatar = randomFileName,
                        CreatedDate = DateTime.Now,
                        UpdatedDate = DateTime.Now
                    };

                    var result = await _userManager.CreateAsync(user, model.Password);

                    if (result.Succeeded){
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return RedirectToAction("Index", "Home");
                    } else {
                        foreach (var err in result.Errors){
                            ModelState.AddModelError(string.Empty, err.Description);
                        }
                    }
                } else {
                    ModelState.AddModelError("", "Bu kullanıcı adı kullanılmaktadır.");
                }
            } else {
                ModelState.AddModelError("", "Bu e-posta adresi kullanılmaktadır.");
            }
        }

        return View(model);
    }

    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if(ModelState.IsValid && model.Email != null && model.Password != null){
            var user = await _userManager.FindByEmailAsync(model.Email);

            if(user != null){
                var result = await _signInManager.PasswordSignInAsync(user, model.Password, isPersistent: true, lockoutOnFailure: true);

                if(result.Succeeded){
                    ViewData["Avatar"] = user.Avatar;
                    return RedirectToAction("Index","Home");
                }
            } else {
                ModelState.AddModelError("", "Email veya şifre yanlış");
            }
        }

        return View(model);
    }

    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index","Home");
    }
}
