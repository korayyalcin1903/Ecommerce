using System.ComponentModel.DataAnnotations;

namespace Ecommerce;

public class RegisterViewModel
{
    [Required]
    public string Name { get; set; }
    [Required]
    public string Surname { get; set; }
    [Required]
    public string UserName { get; set; }
    [Required]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; }
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }
    [Required]
    [DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "Parola eşleşmiyor")]
    public string ConfirmPassword { get; set; }
    public string Avatar { get; set; } = "user.jpg";
    [Required]
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    [Required]
    public DateTime UpdatedDate { get; set; } = DateTime.Now;
}
