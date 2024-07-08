using Ecommerce.Entity;

namespace Ecommerce;

public class Order
{
    public int OrderId { get; set; }
    public DateTime OrderDate { get; set; } = DateTime.Now;
    public DateTime ShippingDate { get; set; }
    public string ShippingAddress { get; set; }
    public string City { get; set; }
    public string Country { get; set; }
    public double TotalAmount { get; set; }
    public string UserId { get; set; }
    public User User{ get; set; }

    public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}

public class OrderItem
{
    public int Id { get; set; }
    public int OrderId { get; set;}
    public Order Order { get; set; }
    public int ProductId { get; set; }
    public Product Product { get; set; }
    public double Price { get; set; }
    public int Quantity { get; set; }

}
