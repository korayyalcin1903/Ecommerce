using System.ComponentModel.DataAnnotations;

namespace Ecommerce;

public class ProfileViewModel
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
    public string Avatar { get; set; } = "user.jpg";
    [Required]
    public string Address { get; set; }
    [Required]
    public string City { get; set; }
    [Required]
    public string Country { get; set; }
    [Required]
    public int PostalCode { get; set; }
    [Required]
    public bool Gender { get; set;}
    [Required]
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    public DateTime UpdatedDate { get; set; } = DateTime.Now;
    
}
