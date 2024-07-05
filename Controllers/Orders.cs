using Ecommerce.Data.Concrete;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce;

public class Orders:Controller
{
    public readonly EcommerceContext _context;
    public Orders(EcommerceContext context)
    {
        _context = context;
    }
    public async Task<IActionResult> Cart()
    {
        return View();
    }

    public async Task<IActionResult> Checkout()
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Name == User.Identity.Name);
        if (user == null){
            return NotFound();
        } else {
            ViewBag.User = user;
        }
        return View();
    }
}
