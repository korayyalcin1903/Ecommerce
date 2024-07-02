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

    public List<ProductOrder> ProductOrders { get; set; } = new List<ProductOrder>();

}
