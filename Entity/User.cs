using Microsoft.AspNetCore.Identity;

namespace Ecommerce;

public class User: IdentityUser
{
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Avatar { get; set; } = "user.jpg";
    public string Address { get; set; }
    public string City { get; set; }
    public string Country { get; set; }
    public int PostalCode { get; set; }
    public bool Gender { get; set;}
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    public DateTime UpdatedDate { get; set; } = DateTime.Now;
    
    public IList<Order> Orders { get; set; } = new List<Order>();
}
