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
                new Product { ProductId = 1, ProductName = "Samsung Galaxy S21 Plus 5G 128 GB (Samsung Türkiye Garantili)", Description = "İyi bir telefon", Price = 50000, Image = "1.jpg", StockQuantity = 5, IsActive = true, CategoryId = 1},
            new Product { ProductId = 2, ProductName = "Samsung Galaxy S22 Plus 5G 128 GB (Samsung Türkiye Garantili)", Description = "İyi bir telefon", Price = 60000, Image = "2.jpg", StockQuantity = 5, IsActive = true, CategoryId = 1},
            new Product { ProductId = 3, ProductName = "Hp Pavilion 15-EH3011NT Amd Ryzen 5 7530U 8gb 512GB SSD 15.6 Fhd Freedos Taşınabilir Bilgisayar 7P6A9EA Ritimtech", Description = "İyi bir bilgisayar", Price = 40000, Image = "3.jpg", StockQuantity = 5, IsActive = true, CategoryId = 2}
            );
            context.SaveChanges();
        }

        if(!context.Users.Any()){
            var hasher = new PasswordHasher<User>();

            var user = new User 
                { Id = "admin" ,Name = "Koray", Surname = "Yalçın", Avatar = "1.jpg", UserName = "korayyalcin", NormalizedUserName = "KORAYYALCIN", Email = "koray@gmail.com", NormalizedEmail="KORAY@GMAIL.COM", Address = "Pendik", City = "İstanbul", Country = "Turkey", PostalCode = 34034, PhoneNumber = "+90555555555555", Gender = true, EmailConfirmed = true};

            user.PasswordHash = hasher.HashPassword(user, "1234");
            context.Users.Add(user);
            context.SaveChanges();
        }

        if(!context.Roles.Any()){
            context.Roles.AddRange(
                new IdentityRole { Id = "admin", Name = "Admin"}
            );
            context.SaveChanges();
        }

        if(!context.UserRoles.Any()){
            context.UserRoles.AddRange(
                new IdentityUserRole<string> { RoleId = "admin", UserId = "admin"}
            );
            context.SaveChanges();
        }
    }
}
