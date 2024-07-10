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
    
    public async Task<IActionResult> Index(string category, string search, int page)
    {
        var pagination = new Pagination();
        var products = await _context.Products.ToListAsync();

        var totalPages = pagination.TotalPages(_context.Products.Count());
        
        ViewBag.TotalPages = totalPages;
        ViewBag.Categories = await _context.Categorys.ToListAsync();

        if(!string.IsNullOrEmpty(category)){
            var categoryEntity = await _context.Categorys.FirstAsync(x => x.CategoryName == category);
            products = products.Where(x => x.CategoryId == categoryEntity.CategoryId).ToList();

            if(!string.IsNullOrEmpty(search)){
            products = await _context.Products.Where(x => x.ProductName.Contains(search)).ToListAsync();
            totalPages = pagination.TotalPages(products.Count());
            ViewBag.TotalPages = totalPages;

            if(page != null){
                if (page < 1){
                    page = 1;
                }
                
                var skipAmount = (page - 1) * pagination.PageSize;
                ViewBag.CurrentPage = page > 1 ? page - 1 : page;
                ViewBag.NextPage = page < totalPages ? page + 1 : page;
                ViewBag.Page = page;
            }
            return View(products);
            }

            totalPages = pagination.TotalPages(products.Count());
            ViewBag.TotalPages = totalPages;

            if(page != null){
                if (page < 1){
                    page = 1;
                }
                
                var skipAmount = (page - 1) * pagination.PageSize;
                products = await _context.Products.Where(x => x.CategoryId == categoryEntity.CategoryId).Skip((int)skipAmount).Take((int)pagination.PageSize).ToListAsync();
                ViewBag.CurrentPage = page > 1 ? page - 1 : page;
                ViewBag.NextPage = page < totalPages ? page + 1 : page;
                ViewBag.Page = page;
            }

            
            return View(products);
        }


        if(!string.IsNullOrEmpty(search)){
            products = await _context.Products.Where(x => x.ProductName.Contains(search)).ToListAsync();
            totalPages = pagination.TotalPages(products.Count());
            ViewBag.TotalPages = totalPages;
            return View(products);
        }


        if(page != null){
           if (page < 1){
                page = 1;
            }

            var skipAmount = (page - 1) * pagination.PageSize;
            products = await _context.Products.Skip((int)skipAmount).Take((int)pagination.PageSize).ToListAsync();
            ViewBag.CurrentPage = page > 1 ? page - 1 : page;
            ViewBag.NextPage = page < totalPages ? page + 1 : page;
            ViewBag.Page = page;

        }

        return View(products);
    }



}
