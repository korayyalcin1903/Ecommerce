using Ecommerce.Entity;

namespace Ecommerce;

public class Category
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; }
    public int? ParentCategoryId { get; set; }
    public virtual Category ParentCategory { get; set; }
    public List<Product> Products{ get; set; } = new List<Product>();
}

