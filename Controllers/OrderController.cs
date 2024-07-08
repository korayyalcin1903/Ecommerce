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
    public async Task<IActionResult> Checkout(int id)
    {
        var cart = HttpContext.Session.GetJson<Cart>("cart") ?? new Cart();
        var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == User.Identity.Name);

        var viewModel = new CartViewModel{
            Cart = cart
        };

        ViewBag.Products = viewModel.Cart.Items;
        ViewBag.Total = viewModel.Cart.CalculateTotal().ToString("C2");

        if(user.Country !=null || user.Address != null || user.City != null){
            ViewBag.User = user;
            if(cart.Items.Count == 0){
                ModelState.AddModelError("", "Sepetinizde ürün yok");
            }
        
            if(ModelState.IsValid){
                var order = new Order {
                    ShippingAddress = user.Address,
                    Country = user.Country,
                    City = user.City,
                    OrderDate = DateTime.Now,
                    ShippingDate = DateTime.Now,
                    TotalAmount = (double)cart.CalculateTotal(),
                    UserId = user.Id,
                    User = user,
                    OrderItems = cart.Items.Select(x => new OrderItem{
                        ProductId = x.Product.ProductId,
                        Price = x.Product.Price,
                        Quantity = x.Quantity,
                    }).ToList()
                };
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();
                cart.Clear();
                HttpContext.Session.SetJson("cart", cart);
                return RedirectToAction("Completed", "Order", new { OrderId = order.OrderId});
            } else {
                return View();
            }
        } else {
            return RedirectToAction("Edit","Account");
        }        
    }

    public async Task<IActionResult> Completed(int OrderId)
    {
        var order = await _context.Orders.FirstOrDefaultAsync(x => x.OrderId == OrderId);
        return View(order);
    }
}
