using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Ecommerce;

public class CheckoutViewModel
{   
    [Required]
    [Display(Name = "Name on Card")]
    public string CardHolderName { get; set; }
    [Required]
    [DisplayName("Card Number")]
    [RegularExpression(@"\d{16}", ErrorMessage = "Card Number must be exactly 16 digits.")]
    public string CardNumber { get; set; }
    [Required]
    [DisplayName("Month")]
    public int ExpirationMonth { get; set; }
    [DisplayName("Year")]
    public int ExpirationYear { get; set; }
    [Required]
    [RegularExpression(@"\d{3}", ErrorMessage = "CVC must be exactly 3 digits.")]
    public int Cvc { get; set; }

    public Cart Cart { get; set; }
}
