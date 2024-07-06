using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Ecommerce;

public class ProductViewModel
{
    [Required]
    [DisplayName("Product Name")]
    public string ProductName { get; set;}
    [Required]
    public string Description { get; set;}
    [Required]
    public double Price { get; set; }
    public string Image { get; set; }
    [Required]
    public int StockQuantity { get; set; }
    public DateTime UpdatedDate { get; set;} = DateTime.Now;
    [Required]
    public bool IsActive { get; set; }
    [Required]
    public int CategoryId { get; set; }
}
