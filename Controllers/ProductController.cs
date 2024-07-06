using Ecommerce.Data.Concrete;
using Ecommerce.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce;

public class ProductController: Controller
{
    public readonly EcommerceContext _context;

    public ProductController(EcommerceContext context)
    {
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
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/product", randomFileName);

                using (var stream = new FileStream(path, FileMode.Create)){
                    await formFile.CopyToAsync(stream);
                }
            }
        }

        return randomFileName;
    }

    public async Task<IActionResult> Details(int id)
    {
        var product = await _context.Products.FirstOrDefaultAsync(x => x.ProductId == id);
        if(product != null){
            ViewBag.Products = _context.Products.Where(x => x.CategoryId == product.CategoryId && x.ProductId != id).Take(5);
            return View(product);
        } else {
            return NotFound();
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        ViewData["Category"] = await _context.Categorys.ToListAsync();

        return View();
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create(ProductViewModel model, IFormFile formFile)
    {
        ViewData["Category"] = await _context.Categorys.ToListAsync();

        var extension = "";
        var randomFileName = "";

        if (formFile != null){
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            extension = Path.GetExtension(formFile.FileName);

            if (!allowedExtensions.Contains(extension)){
                ModelState.AddModelError("", "Geçerli bir resim seçiniz.");
            } else {
                randomFileName = $"{Guid.NewGuid()}{extension}";
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/product", randomFileName);

                using (var stream = new FileStream(path, FileMode.Create)){
                    await formFile.CopyToAsync(stream);
                }
            }
        }

        if(ModelState.IsValid) {
            await _context.Products.AddAsync( new Product {
                ProductName = model.ProductName,
                Description = model.Description,    
                Price = model.Price,
                Image = randomFileName,
                StockQuantity = model.StockQuantity,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now,
                IsActive = false,
                CategoryId = model.CategoryId,
            });

            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        } else {
            return View(model);
        }

    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var product = await _context.Products.FirstOrDefaultAsync(x => x.ProductId == id);

        ViewBag.Category = await _context.Categorys.ToListAsync();

        if(product != null){
            var model = new ProductViewModel {
                ProductName = product.ProductName,
                Description = product.Description,
                CategoryId = product.CategoryId,
                Image = product.Image,
                IsActive = product.IsActive,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                UpdatedDate = product.UpdatedDate,
            };

            return View(model);
        } else {
            return NotFound();
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Edit(ProductViewModel model, int id, IFormFile formFile)
    {
        var product = await _context.Products.FirstOrDefaultAsync(x => x.ProductId == id);

        if(product != null && model != null){
            product.ProductName = model.ProductName;
            product.Description = model.Description;
            product.CategoryId = model.CategoryId;
            product.Image = await ImageUpload(formFile, product.Image);
            product.UpdatedDate = DateTime.Now;
            product.Price = model.Price;
            product.StockQuantity = model.StockQuantity;
            product.IsActive = model.IsActive;
        }

        await _context.SaveChangesAsync();
        return RedirectToAction("Details", "Product", new { id = product.ProductId });
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _context.Products.FirstAsync(x => x.ProductId == id);

        if(product != null){
            return View(product);
        } else {
            return NotFound();
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Delete(int id, string productId)
    {
        if(id == null){
            return NotFound();
        }

        var product = await _context.Products.FirstAsync(x => x.ProductId == id);

        if(product != null){
            _context.Remove(product);
            _context.SaveChanges();
            return RedirectToAction("Index", "Home");
        } else {
            return NotFound();
        }
    }
}
