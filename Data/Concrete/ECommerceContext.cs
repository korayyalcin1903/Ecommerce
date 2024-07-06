using Ecommerce.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Data.Concrete
{
    public class EcommerceContext: IdentityDbContext<User>
    {
        public EcommerceContext(DbContextOptions<EcommerceContext> options): base(options)
        {}

        public new virtual DbSet<User> Users => Set<User>();
        public virtual DbSet<Product> Products => Set<Product>();
        public virtual DbSet<Order> Orders => Set<Order>();
        public virtual DbSet<Category> Categorys => Set<Category>();
        public virtual DbSet<ProductOrder> ProductOrders=> Set<ProductOrder>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ProductOrder>()
                .HasKey(po => new { po.ProductId, po.OrderId });

            // ProductOrder: Relationships Configuration
            modelBuilder.Entity<ProductOrder>()
                .HasOne(po => po.Product)
                .WithMany(p => p.ProductOrders)
                .HasForeignKey(po => po.ProductId);

            modelBuilder.Entity<ProductOrder>()
                .HasOne(po => po.Order)
                .WithMany(o => o.ProductOrders)
                .HasForeignKey(po => po.OrderId);

            // User: Relationship with Order
            modelBuilder.Entity<User>()
                .HasMany(u => u.Orders)
                .WithOne(o => o.User)
                .HasForeignKey(o => o.UserId);

            // Product: Relationship with Category
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId);

        }
    }
}