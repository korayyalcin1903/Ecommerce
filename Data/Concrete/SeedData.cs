using Ecommerce.Data.Concrete;
using Ecommerce.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce;

public static class SeedData
{
    public static void TestVerileriniDoldur(IApplicationBuilder app)
    {
        var context = app.ApplicationServices.CreateScope().ServiceProvider.GetService<EcommerceContext>();

        if(context != null){
            context.Database.Migrate();
        }

        if(!context.Categorys.Any()){
            context.Categorys.AddRange(
                new Category { CategoryId = 1, CategoryName = "Telefon"},
                new Category { CategoryId = 2, CategoryName = "Bilgisayar"}
            );
            context.SaveChanges();
        }

        if(!context.Products.Any()){
            context.Products.AddRange(
                new Product { ProductId = 1, ProductName = "Samsung S21", Description = "İyi bir telefon", Price = 50000, Image = "1.jpg", StockQuantity = 5, IsActive = true, CategoryId = 1},
            new Product { ProductId = 2, ProductName = "Samsung S22", Description = "İyi bir telefon", Price = 60000, Image = "2.jpg", StockQuantity = 5, IsActive = true, CategoryId = 1},
            new Product { ProductId = 3, ProductName = "HP Pavilion 1", Description = "İyi bir bilgisayar", Price = 40000, Image = "3.jpg", StockQuantity = 5, IsActive = true, CategoryId = 2}
            );
            context.SaveChanges();
        }

        if(!context.Users.Any()){
            var hasher = new PasswordHasher<User>();

            var user = new User 
                { Name = "Koray", Surname = "Yalçın", Avatar = "1.jpg", UserName = "korayyalcin", NormalizedUserName = "KORAYYALCIN", Email = "koray@gmail.com", NormalizedEmail="KORAY@GMAIL.COM", Address = "Pendik", City = "İstanbul", Country = "Turkey", PostalCode = 34034, PhoneNumber = "+90555555555555", Gender = true};

            user.PasswordHash = hasher.HashPassword(user, "1234");
            context.Users.Add(user);
            context.SaveChanges();
        }
    }
}
