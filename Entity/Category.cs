using Ecommerce.Entity;

namespace Ecommerce;

public class Category
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; }
    public int? CategoryCategoryId { get; set;} = null;

    public List<Product> Products{ get; set; } = new List<Product>();
}
