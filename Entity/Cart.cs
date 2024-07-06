using Ecommerce.Entity;

namespace Ecommerce;

public class Cart
{
    public List<CartItem> Items { get; set; } = new List<CartItem>();
    public void AddItem(Product product, int quantity)
    {
        var item = Items.Where(p => p.Product.ProductId == product.ProductId).FirstOrDefault();

        if(item == null){
            Items.Add(new CartItem {Product = product, Quantity = quantity});
        } else {
            item.Quantity += quantity;
        }
    }
    public void RemoveItem(Product product)
    {
        Items.RemoveAll(i => i.Product.ProductId == product.ProductId);
    }
    public decimal CalculateTotal()
    {
        return (decimal)Items.Sum(i => i.Product.Price * i.Quantity);
    }

    public void Clear()
    {
        Items.Clear();
    }
}

public class CartItem
{
    public int CartItemId { get; set; }
    public Product Product{ get; set; } = new Product();
    public int Quantity { get; set; }
}
