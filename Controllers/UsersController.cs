using Ecommerce.Data.Concrete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce;

public class UsersController:Controller
{
    public readonly EcommerceContext _context;
    public UsersController(EcommerceContext context){
        _context = context;
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Index()
    {
        var user = await _context.Users.ToListAsync();
        return View(user);
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(string id)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
        if (user != null){
            return View(user);
        } else {
            return NotFound();
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(string id, string names)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
        var orders = await _context.Orders.Where(x => x.UserId == id).ToListAsync();
        if (user != null){
            if (user.Orders != null){
                _context.Orders.RemoveRange(orders);
            }
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index","Users");
        } else {
            return NotFound();
        }
    }
}
