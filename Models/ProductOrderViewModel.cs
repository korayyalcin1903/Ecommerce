using Ecommerce.Entity;

namespace Ecommerce;

public class ProductOrderViewModel
{
    public Order Order { get; set; }
    public List<Product> Products{ get; set; }
    public List<OrderItem> OrderItems { get; set; }
}
