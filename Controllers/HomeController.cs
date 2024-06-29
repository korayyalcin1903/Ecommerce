using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Ecommerce.Models;
using Ecommerce.Data.Concrete;
using Ecommerce.Entity;

namespace Ecommerce.Controllers;

public class HomeController : Controller
{
    public readonly EcommerceContext _context;

    public HomeController(EcommerceContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var product = _context.Products.ToList();
        return View(product);
    }

}
