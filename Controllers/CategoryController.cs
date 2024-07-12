using Ecommerce.Data.Concrete;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce;

public class CategoryController:Controller
{
    public readonly EcommerceContext _context;
    public CategoryController(EcommerceContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var categories = await _context.Categorys.ToListAsync();
        return View(categories);
    }

    public async Task<IActionResult> Details(int id)
    {
        var products = await _context.Products.Where(x =>  x.CategoryId == id).ToListAsync();
        var categoryName = await _context.Categorys.FirstOrDefaultAsync(x => x.CategoryId == id);
        ViewBag.Category = categoryName.CategoryName.ToString();
        return View(products);
    }
}
