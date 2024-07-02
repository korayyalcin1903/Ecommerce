
using Ecommerce;
using Ecommerce.Data.Concrete;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


builder.Services.AddDbContext<EcommerceContext>(options => 
    options.UseMySql(
        builder.Configuration.GetConnectionString("mysql_connection"),
        new MySqlServerVersion(new Version(8, 0 , 29))
    )
);

builder.Services.Configure<IdentityOptions>(options =>
{
    // Password settings.
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 3;
    options.Password.RequiredUniqueChars = 1;

    // User settings.
    options.User.AllowedUserNameCharacters =
    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = false;
});

builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<EcommerceContext>()
    .AddDefaultTokenProviders();

var app = builder.Build();

SeedData.TestVerileriniDoldur(app);


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "product_by_category",
    pattern: "/Category/{category}",
    defaults: new {controller = "Home", action = "Index"}
);

app.MapControllerRoute(
    name: "account_by_profile",
    pattern: "/Profile/{username}",
    defaults: new {controller = "Account", action = "Profile"}
);

app.MapControllerRoute(
    name: "products_by_details",
    pattern: "/Details/{id}",
    defaults: new {controller = "Product", action = "Details"}
);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
