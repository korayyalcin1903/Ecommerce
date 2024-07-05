using Microsoft.AspNetCore.Mvc;

namespace Ecommerce;

public class Orders:Controller
{
    public async Task<IActionResult> Cart()
    {
        return View();
    }

    public async Task<IActionResult> Checkout()
    {
        return View();
    }
}
