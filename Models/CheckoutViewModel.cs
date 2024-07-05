using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Ecommerce;

public class CheckoutViewModel
{
    [Required]
    [DisplayName("Card Number")]
    [RegularExpression(@"\d{16}", ErrorMessage = "Card Number must be exactly 16 digits.")]
    public int CardNumber { get; set; }
    [Required]
    [DisplayName("Expiration date")]
    public DateTime ExpirationDate { get; set; }
    [Required]
    [RegularExpression(@"\d{3}", ErrorMessage = "CVC must be exactly 3 digits.")]
    public int Cvc { get; set; }
}
