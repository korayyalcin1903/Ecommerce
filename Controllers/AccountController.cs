using Ecommerce.Data.Concrete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Controllers;

public class AccountController:Controller
{
    public readonly UserManager<User> _userManager;
    public readonly SignInManager<User> _signInManager;
    public readonly EcommerceContext _context;
    public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, EcommerceContext context)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _context = context;
    }

    public async Task<string> ImageUpload(IFormFile formFile, string currentAvatarFileName)
    {
        var extension = "";
        var randomFileName = currentAvatarFileName;

        if (formFile != null){
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            extension = Path.GetExtension(formFile.FileName);

            if (!allowedExtensions.Contains(extension)){
                ModelState.AddModelError("", "Geçerli bir resim seçiniz.");
            } else {
                randomFileName = $"{Guid.NewGuid()}{extension}";
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/user", randomFileName);

                using (var stream = new FileStream(path, FileMode.Create)){
                    await formFile.CopyToAsync(stream);
                }
            }
        }

        return randomFileName;
    }

    [Authorize]
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
                        Avatar = await ImageUpload(formFile, model.Avatar),
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

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Edit()
    {
        var username = User.Identity.Name;
        if (username != null){
            var user = await _userManager.FindByNameAsync(username);
            if (user != null){
                var model = new ProfileViewModel{
                    Name = user.Name,
                    Surname = user.Surname,
                    UserName = user.UserName,
                    Email = user.Email,
                    Avatar = user.Avatar,
                    Address = user.Address,
                    City = user.City,
                    Country = user.Country,
                    Gender = user.Gender,
                    PostalCode = user.PostalCode
                };

                return View(model);
            }
        }
        return NotFound();
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Edit(ProfileViewModel model, IFormFile formFile)
    {

        if(ModelState.IsValid){
            if(model.Name == null){
                return NotFound();
            } else {
                var username = User.Identity.Name;
                if(username != null){
                    var user = await _userManager.FindByNameAsync(username);
                    if(user != null){
                        user.Name = model.Name;
                        user.Surname = model.Surname;
                        user.UserName = model.UserName;
                        user.Email = model.Email;
                        user.Avatar = await ImageUpload(formFile, user.Avatar);
                        user.Address = model.Address;
                        user.City = model.City;
                        user.Country = model.Country;
                        user.PostalCode = model.PostalCode;
                        user.Gender = model.Gender;
                        user.UpdatedDate = DateTime.Now;

                        var result = await _userManager.UpdateAsync(user);
                        if(result.Succeeded){
                            return RedirectToAction("Profile","Account", new {username = user.UserName});
                        } else {
                            foreach(var err in result.Errors){
                                ModelState.AddModelError(string.Empty, err.Description);
                            }
                        }
                    } else {
                        return View(model);
                    }
                } else {
                    return View(model);
                }
            }
        }
        return View(model);
    }

    public async Task<IActionResult> Order()
    {
        var user = await _userManager.GetUserAsync(User);
        var order = await _context.Orders.Where(x => x.UserId == user.Id).ToListAsync();

        if(order.Count > 0){
            return View(order);
        }

        return View();
    }

    [HttpGet]
    public async Task<IActionResult> OrderDetails(int id){
        var order = await _context.Orders.FirstOrDefaultAsync(x => x.OrderId == id);
        var orderItems = await _context.OrderItems.Where(x => x.OrderId == id).ToListAsync();
        var products = await _context.Products.ToListAsync();

        var viewModel = new ProductOrderViewModel{
            Order = order,
            Products = products,
            OrderItems = orderItems
        };
        
        if(order != null){
            return View(viewModel);
        } else {
            return NotFound();
        }
    }
}
