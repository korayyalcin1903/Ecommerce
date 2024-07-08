using Ecommerce.Data.Concrete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce;

public class OrderController:Controller
{
    public readonly EcommerceContext _context;
    public OrderController(EcommerceContext context)
    {
        _context = context;
    }
    public IActionResult Cart()
    {
        var cart = HttpContext.Session.GetJson<Cart>("cart") ?? new Cart();
            var viewModel = new CartViewModel{
                Cart = cart
            };
            return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Cart(int id)
    {
        var product = await _context.Products.FirstOrDefaultAsync(i => i.ProductId == id);

            if (product != null){
                var cart = HttpContext.Session.GetJson<Cart>("cart") ?? new Cart();
                cart.AddItem(product, 1);
                HttpContext.Session.SetJson("cart", cart);
            }

            return RedirectToAction("Cart");
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _context.Products.FirstOrDefaultAsync(i => i.ProductId == id);
        if(product != null){
            var cart = HttpContext.Session.GetJson<Cart>("cart") ?? new Cart();
            cart.DecreaseItem(product);
            HttpContext.Session.SetJson("cart", cart);
        }

        return RedirectToAction("Cart");
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Checkout()
    {
        var cart = HttpContext.Session.GetJson<Cart>("cart") ?? new Cart();
        var viewModel = new CartViewModel{
            Cart = cart
        };

        ViewBag.Products = viewModel.Cart.Items;
        ViewBag.Total = viewModel.Cart.CalculateTotal().ToString("C2");

        var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == User.Identity.Name);

        if (user == null){
            return NotFound();
        } else {
            ViewBag.User = user;
        }
        return View();
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Checkout(CheckoutViewModel model)
    {
        return View();
    }
}
