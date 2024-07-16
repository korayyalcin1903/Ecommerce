using Ecommerce.Data.Concrete;
using Microsoft.AspNetCore.Authorization;
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

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Index()
    {
        var categories = await _context.Categorys.ToListAsync();
        return View(categories);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Index(string categoryName)
    {
        var categories = await _context.Categorys.ToListAsync();
        var category = await _context.Categorys.FirstOrDefaultAsync(x => x.CategoryName == categoryName);

        if(ModelState.IsValid){
            if(category == null){
                await _context.Categorys.AddAsync( new Category {CategoryName = categoryName});
                await _context.SaveChangesAsync();
                return RedirectToAction("Index","Category");
            } else {
                ModelState.AddModelError("", "Kategori zaten var");
            }
        }        
        return View(categories);
    }
    
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Details(int id)
    {
        var products = await _context.Products.Where(x =>  x.CategoryId == id).ToListAsync();
        var categoryName = await _context.Categorys.FirstOrDefaultAsync(x => x.CategoryId == id);
        ViewBag.Category = categoryName.CategoryName.ToString();
        return View(products);
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var category = await _context.Categorys.FirstOrDefaultAsync(x => x.CategoryId == id);
        if(category != null){
            return View(category);
        } else {
            return NotFound();
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int? id)
    {
        var category = await _context.Categorys.FirstOrDefaultAsync(x => x.CategoryId == id);
        if(category != null){
            _context.Remove(category);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index","Category");
        } else {
            return NotFound();
        }
    }

}
