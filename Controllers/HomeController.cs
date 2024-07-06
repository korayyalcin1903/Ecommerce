using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Ecommerce.Models;
using Ecommerce.Data.Concrete;
using Ecommerce.Entity;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Controllers;

public class HomeController : Controller
{
    public readonly EcommerceContext _context;

    public HomeController(EcommerceContext context)
    {
        _context = context;
    }
    
    public async Task<IActionResult> Index(string category, string search)
    {
        var products = await _context.Products.ToListAsync();
        ViewBag.Categories = await _context.Categorys.ToListAsync();

        if(!string.IsNullOrEmpty(category)){
            var categoryEntity = await _context.Categorys.FirstAsync(x => x.CategoryName == category);
            products = products.Where(x => x.CategoryId == categoryEntity.CategoryId).ToList();

            return View(products);
        }

        if(!string.IsNullOrEmpty(search)){
            products = await _context.Products.Where(x => x.ProductName.Contains(search)).ToListAsync();
        }

        return View(products);
    }



}
