namespace Ecommerce.Entity
{
    public class Product()
    {
        public int ProductId { get; set; }
        public string ProductName { get; set;}
        public string Description { get; set;}
        public double Price { get; set; }
        public string Image { get; set; }
        public int StockQuantity { get; set; }
        public DateTime CreatedDate { get; set;} = DateTime.Now;
        public DateTime UpdatedDate { get; set;} = DateTime.Now;
        public bool IsActive { get; set; }

        public int CategoryId { get; set; }
        public Category Category{ get; set; } = null;

        public List<Order> ProductOrders { get; set; } = new List<Order>();

    }
}